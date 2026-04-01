using Microsoft.EntityFrameworkCore;
using src.Models.Mappers;
using src.Repositories.Interfaces;
using Dom = src.Models.Domain;
using EF = src.Models.CodeFirst;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace src.Repositories.Implementations
{
    /// <summary>
    /// Maneja la lógica de dónde está físicamente cada producto (N-N Producto-Fila con cantidad)
    /// y los movimientos de stock entre filas.
    /// </summary>
    public class UbicacionProductoRepository : IUbicacionProductoRepository
    {
        private readonly EF.AppDbContext _context;
        private readonly UbicacionProductoMapper _ubicacionMapper;

        public UbicacionProductoRepository(EF.AppDbContext context, UbicacionProductoMapper ubicacionMapper)
        {
            _context = context;
            _ubicacionMapper = ubicacionMapper;
        }

        // ─────────────────────────────────────────────
        // CONSULTAS
        // ─────────────────────────────────────────────

        /// <summary>
        /// Devuelve todas las ubicaciones de un producto dado.
        /// </summary>
        public async Task<IEnumerable<Dom.UbicacionProducto>> GetByProductoAsync(int idProducto)
        {
            var efUbicaciones = await _context.UbicacionesProductos
                .Where(up => up.IdProducto == idProducto)
                .Include(up => up.Fila)
                    .ThenInclude(f => f.Estante)
                        .ThenInclude(e => e.Deposito)
                .AsNoTracking()
                .ToListAsync();

            return efUbicaciones.Select(up => _ubicacionMapper.ToDomainSinProducto(up));
        }

        /// <summary>
        /// Devuelve todos los productos ubicados en una fila dada.
        /// </summary>
        public async Task<IEnumerable<Dom.UbicacionProducto>> GetByFilaAsync(int idFila)
        {
            var efUbicaciones = await _context.UbicacionesProductos
                .Where(up => up.IdFila == idFila)
                .Include(up => up.Producto)
                .Include(up => up.Fila)
                    .ThenInclude(f => f.Estante)
                        .ThenInclude(e => e.Deposito)
                .AsNoTracking()
                .ToListAsync();

            return efUbicaciones.Select(up => _ubicacionMapper.ToDomain(up));
        }

        // ─────────────────────────────────────────────
        // ASIGNAR / ACTUALIZAR CANTIDAD EN UBICACIÓN
        // ─────────────────────────────────────────────

        /// <summary>
        /// Asigna una cantidad de un producto a una fila. Si ya existe la relación,
        /// suma la cantidad. Si no existe, la crea.
        /// También actualiza el StockTotal del producto y el TieneEspacio de la fila.
        /// </summary>
        public async Task AgregarStockEnFilaAsync(
    int idProducto, int idFila, decimal cantidad, int idUsuario)
    {
        if (cantidad <= 0)
            throw new ArgumentException("La cantidad debe ser mayor a cero.");

        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var ubicacion = await _context.UbicacionesProductos
                .FirstOrDefaultAsync(up =>
                    up.IdProducto == idProducto && up.IdFila == idFila);

            if (ubicacion is null)
            {
                _context.UbicacionesProductos.Add(new EF.UbicacionProducto
                {
                    IdProducto = (short)idProducto,
                    IdFila     = idFila,
                    Cantidad   = cantidad,
                    Fila       = null!,    // ← fix
                    Producto   = null!     // ← fix
                });
            }
            else
            {
                ubicacion.Cantidad += cantidad;
            }

            var producto = await _context.Productos.FindAsync((short)idProducto)
                ?? throw new KeyNotFoundException($"Producto {idProducto} no encontrado.");
            producto.StockTotal += (int)cantidad;

            _context.MovimientosStock.Add(new EF.MovimientoStock
            {
                IdProducto      = (short)idProducto,
                IdFilaDestino   = idFila,
                Cantidad        = cantidad,
                FechaMovimiento = DateTime.UtcNow,
                IdUsuario       = (short)idUsuario,
                Producto        = null!,     // ← fix
                FilaOrigen      = null!,     // ← fix
                FilaDestino     = null!,     // ← fix
                Usuario         = null!      // ← fix
            });

            await ValidarEspacioFilaAsync(idFila);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task RetirarStockDeFilaAsync(
        int idProducto, int idFila, decimal cantidad, int idUsuario)
    {
        if (cantidad <= 0)
            throw new ArgumentException("La cantidad debe ser mayor a cero.");

        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var ubicacion = await _context.UbicacionesProductos
                .FirstOrDefaultAsync(up =>
                    up.IdProducto == idProducto && up.IdFila == idFila)
                ?? throw new InvalidOperationException(
                    $"El producto no tiene stock en la fila {idFila}.");

            if (ubicacion.Cantidad < cantidad)
                throw new InvalidOperationException(
                    $"Stock insuficiente. Disponible: {ubicacion.Cantidad:N2}, " +
                    $"solicitado: {cantidad:N2}.");

            ubicacion.Cantidad -= cantidad;
            if (ubicacion.Cantidad == 0)
                _context.UbicacionesProductos.Remove(ubicacion);

            var producto = await _context.Productos.FindAsync((short)idProducto)
                ?? throw new KeyNotFoundException($"Producto {idProducto} no encontrado.");
            producto.StockTotal -= (int)cantidad;

            _context.MovimientosStock.Add(new EF.MovimientoStock
            {
                IdProducto      = (short)idProducto,
                IdFilaOrigen    = idFila,
                Cantidad        = cantidad,
                FechaMovimiento = DateTime.Now,
                IdUsuario       = (short)idUsuario,
                Producto        = null!,    // ← fix
                FilaOrigen      = null!,    // ← fix
                FilaDestino     = null!,    // ← fix
                Usuario         = null!     // ← fix
            });

            await ValidarEspacioFilaAsync(idFila);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
}
        /// <summary>
        /// Mueve una cantidad de producto de una fila origen a una fila destino.
        /// Valida que haya stock suficiente en origen.
        /// </summary>
    public async Task MoverStockAsync(
    int idProducto, int idFilaOrigen, int idFilaDestino,
    decimal cantidad, int idUsuario)
    {
        if (cantidad <= 0)
            throw new ArgumentException("La cantidad debe ser mayor a cero.");
        if (idFilaOrigen == idFilaDestino)
            throw new InvalidOperationException(
                "La fila origen y destino no pueden ser la misma.");

        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // 1. Validar stock en origen
            var ubicacionOrigen = await _context.UbicacionesProductos
                .FirstOrDefaultAsync(up =>
                    up.IdProducto == idProducto && up.IdFila == idFilaOrigen)
                ?? throw new InvalidOperationException(
                    $"El producto no tiene stock en la fila origen (ID {idFilaOrigen}).");

            if (ubicacionOrigen.Cantidad < cantidad)
                throw new InvalidOperationException(
                    $"Stock insuficiente en fila origen. " +
                    $"Disponible: {ubicacionOrigen.Cantidad:N2}, solicitado: {cantidad:N2}.");

            // 2. Restar en origen
            ubicacionOrigen.Cantidad -= cantidad;
            if (ubicacionOrigen.Cantidad == 0)
                _context.UbicacionesProductos.Remove(ubicacionOrigen);

            // 3. Sumar en destino
            var ubicacionDestino = await _context.UbicacionesProductos
                .FirstOrDefaultAsync(up =>
                    up.IdProducto == idProducto && up.IdFila == idFilaDestino);

            if (ubicacionDestino is null)
            {
                _context.UbicacionesProductos.Add(new EF.UbicacionProducto
                {
                    IdProducto = (short)idProducto,
                    IdFila     = idFilaDestino,
                    Cantidad   = cantidad,
                    // ↓ CRÍTICO: nullear navegaciones para que EF no intente insertar
                    //   entidades vacías (= new Fila(), = new Producto(), etc.)
                    Fila       = null!,
                    Producto   = null!
                });
            }
            else
            {
                ubicacionDestino.Cantidad += cantidad;
            }

            // 4. Registrar movimiento — ídem: nullear navegaciones
            _context.MovimientosStock.Add(new EF.MovimientoStock
            {
                IdProducto      = (short)idProducto,
                IdFilaOrigen    = idFilaOrigen,
                IdFilaDestino   = idFilaDestino,
                Cantidad        = cantidad,
                FechaMovimiento =  DateTime.Now,
                IdUsuario       = (short)idUsuario,
                // ↓ CRÍTICO: sin esto EF intenta insertar Fila/Producto/Usuario vacíos
                Producto        = null!,
                FilaOrigen      = null!,
                FilaDestino     = null!,
                Usuario         = null!
            });

            // 5. Validar espacio del destino
            await ValidarEspacioFilaAsync(idFilaDestino);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }


        // ─────────────────────────────────────────────
        // HISTORIAL DE MOVIMIENTOS
        // ─────────────────────────────────────────────

        /// <summary>
        /// Devuelve el historial de movimientos de stock de un producto.
        /// </summary>
        public async Task<IEnumerable<Dom.MovimientoStock>> GetMovimientosAsync(
        int? idProducto = null, int? idDeposito = null)
    {
        var query = _context.MovimientosStock
            .Include(m => m.Producto)
            .Include(m => m.FilaOrigen)
                .ThenInclude(f => f.Estante).ThenInclude(e => e.Deposito)
            .Include(m => m.FilaDestino)
                .ThenInclude(f => f.Estante).ThenInclude(e => e.Deposito)
            .Include(m => m.Usuario)
            .AsNoTracking()
            .AsQueryable();

        if (idProducto.HasValue)
            query = query.Where(m => m.IdProducto == idProducto.Value);

        if (idDeposito.HasValue)
            query = query.Where(m =>
                (m.FilaOrigen  != null && m.FilaOrigen.Estante.IdDeposito  == idDeposito.Value) ||
                (m.FilaDestino != null && m.FilaDestino.Estante.IdDeposito == idDeposito.Value));

        var lista = await query
            .OrderByDescending(m => m.FechaMovimiento)
            .ToListAsync();

        return lista.Select(MapMovimiento);
    }
    private static Dom.MovimientoStock MapMovimiento(EF.MovimientoStock m) => new()
    {
        IdMovimientoStock = m.IdMovimientoStock,
        Cantidad          = m.Cantidad,
        FechaMovimiento   = m.FechaMovimiento,
        Producto    = m.Producto is null ? new() : new Dom.Producto  { Nombre = m.Producto.Nombre },
        FilaOrigen  = MapFila(m.FilaOrigen),
        FilaDestino = MapFila(m.FilaDestino),
        Usuario     = m.Usuario is null  ? new() : new Dom.Usuario   { Nombre = m.Usuario.Nombre }
    };  

    private static Dom.Fila MapFila(EF.Fila? fila)
    {
        if (fila is null)  return null; 
        return new Dom.Fila
        {
            IdFila = fila.IdFila,
            NFila  = fila.NFila,
            Estante = fila.Estante is null ? new() : new Dom.Estante
            {
                NumeroEstante = fila.Estante.NumeroEstante,
                Deposito = fila.Estante.Deposito is null ? new() : new Dom.Deposito
                {
                    IdDeposito = fila.Estante.Deposito.IdDeposito,
                    Nombre     = fila.Estante.Deposito.Nombre
                }
            }
        };
    }

        // ─────────────────────────────────────────────
        // HELPERS PRIVADOS
        // ─────────────────────────────────────────────

        /// <summary>
        /// Recalcula si una fila tiene espacio sumando el stock de todos sus productos.
        /// Ajusta la lógica según tu regla de negocio (ej: capacidad máxima, etc.)
        /// </summary>
        private async Task ValidarEspacioFilaAsync(int idFila)
        {
            var fila = await _context.Filas
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.IdFila == idFila)
                ?? throw new KeyNotFoundException($"Fila {idFila} no encontrada.");

            if (!fila.TieneEspacio)
                throw new InvalidOperationException(
                    $"La fila {fila.NFila} (ID {idFila}) está marcada como sin espacio disponible.");
        }
    }
}