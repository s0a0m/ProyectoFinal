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
        public async Task AgregarStockEnFilaAsync(int idProducto, int idFila, decimal cantidad, int idUsuario)
        {
            if (cantidad <= 0)
                throw new ArgumentException("La cantidad debe ser mayor a cero.");

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Ubicación existente o nueva
                var ubicacion = await _context.UbicacionesProductos
                    .FirstOrDefaultAsync(up => up.IdProducto == idProducto && up.IdFila == idFila);

                if (ubicacion == null)
                {
                    ubicacion = new EF.UbicacionProducto
                    {
                        IdProducto = (short)idProducto,
                        IdFila = idFila,
                        Cantidad = cantidad
                    };
                    _context.UbicacionesProductos.Add(ubicacion);
                }
                else
                {
                    ubicacion.Cantidad += cantidad;
                }

                // 2. Actualizar StockTotal del producto
                var producto = await _context.Productos.FindAsync((short)idProducto)
                    ?? throw new KeyNotFoundException($"Producto {idProducto} no encontrado.");
                producto.StockTotal += (int)cantidad;

                // 3. Registrar movimiento de stock (entrada: FilaOrigen = 0/null, destino = idFila)
                _context.MovimientosStock.Add(new EF.MovimientoStock
                {
                    IdProducto = (short)idProducto,
                    IdFilaDestino = idFila,
                    Cantidad = cantidad,
                    FechaMovimiento = DateTime.UtcNow,
                    IdUsuario = (short)idUsuario
                });

                // 4. Actualizar TieneEspacio de la fila (lógica simple; ajusta según tu modelo)
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
        public async Task MoverStockAsync(int idProducto, int idFilaOrigen, int idFilaDestino, decimal cantidad, int idUsuario)
        {
            if (cantidad <= 0)
                throw new ArgumentException("La cantidad debe ser mayor a cero.");

            if (idFilaOrigen == idFilaDestino)
                throw new InvalidOperationException("La fila origen y destino no pueden ser la misma.");

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Validar stock en origen
                var ubicacionOrigen = await _context.UbicacionesProductos
                    .FirstOrDefaultAsync(up => up.IdProducto == idProducto && up.IdFila == idFilaOrigen)
                    ?? throw new InvalidOperationException($"El producto {idProducto} no tiene stock en la fila {idFilaOrigen}.");

                if (ubicacionOrigen.Cantidad < cantidad)
                    throw new InvalidOperationException(
                        $"Stock insuficiente en fila {idFilaOrigen}. Disponible: {ubicacionOrigen.Cantidad}, solicitado: {cantidad}.");

                // 2. Restar en origen
                ubicacionOrigen.Cantidad -= cantidad;
                if (ubicacionOrigen.Cantidad == 0)
                    _context.UbicacionesProductos.Remove(ubicacionOrigen);

                // 3. Sumar en destino
                var ubicacionDestino = await _context.UbicacionesProductos
                    .FirstOrDefaultAsync(up => up.IdProducto == idProducto && up.IdFila == idFilaDestino);

                if (ubicacionDestino == null)
                {
                    _context.UbicacionesProductos.Add(new EF.UbicacionProducto
                    {
                        IdProducto = (short)idProducto,
                        IdFila = idFilaDestino,
                        Cantidad = cantidad
                    });
                }
                else
                {
                    ubicacionDestino.Cantidad += cantidad;
                }

                // 4. El StockTotal del producto NO cambia (solo cambia de lugar).

                // 5. Registrar movimiento
                _context.MovimientosStock.Add(new EF.MovimientoStock
                {
                    IdProducto = (short)idProducto,
                    IdFilaOrigen = idFilaOrigen,
                    IdFilaDestino = idFilaDestino,
                    Cantidad = cantidad,
                    FechaMovimiento = DateTime.UtcNow,
                    IdUsuario = (short)idUsuario
                });

                // 6. Actualizar TieneEspacio de ambas filas
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

        /// <summary>
        /// Retira (egresa) stock de una fila específica. Reduce el StockTotal del producto.
        /// </summary>
        public async Task RetirarStockDeFilaAsync(int idProducto, int idFila, decimal cantidad, int idUsuario)
        {
            if (cantidad <= 0)
                throw new ArgumentException("La cantidad debe ser mayor a cero.");

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var ubicacion = await _context.UbicacionesProductos
                    .FirstOrDefaultAsync(up => up.IdProducto == idProducto && up.IdFila == idFila)
                    ?? throw new InvalidOperationException($"El producto {idProducto} no tiene stock en la fila {idFila}.");

                if (ubicacion.Cantidad < cantidad)
                    throw new InvalidOperationException(
                        $"Stock insuficiente. Disponible: {ubicacion.Cantidad}, solicitado: {cantidad}.");

                ubicacion.Cantidad -= cantidad;
                if (ubicacion.Cantidad == 0)
                    _context.UbicacionesProductos.Remove(ubicacion);

                // Reducir StockTotal
                var producto = await _context.Productos.FindAsync((short)idProducto)
                    ?? throw new KeyNotFoundException($"Producto {idProducto} no encontrado.");
                producto.StockTotal -= (int)cantidad;

                // Registrar movimiento (egreso: solo origen)
                _context.MovimientosStock.Add(new EF.MovimientoStock
                {
                    IdProducto = (short)idProducto,
                    IdFilaOrigen = idFila,
                    Cantidad = cantidad,
                    FechaMovimiento = DateTime.UtcNow,
                    IdUsuario = (short)idUsuario
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

        // ─────────────────────────────────────────────
        // HISTORIAL DE MOVIMIENTOS
        // ─────────────────────────────────────────────

        /// <summary>
        /// Devuelve el historial de movimientos de stock de un producto.
        /// </summary>
        public async Task<IEnumerable<Dom.MovimientoStock>> GetMovimientosByProductoAsync(int idProducto)
        {
            // Asumimos que tienes un MovimientoStockMapper; si no, mapea manualmente.
            var movimientos = await _context.MovimientosStock
                .Where(m => m.IdProducto == idProducto)
                .Include(m => m.Producto)
                .Include(m => m.FilaOrigen).ThenInclude(f => f.Estante).ThenInclude(e => e.Deposito)
                .Include(m => m.FilaDestino).ThenInclude(f => f.Estante).ThenInclude(e => e.Deposito)
                .Include(m => m.Usuario)
                .OrderByDescending(m => m.FechaMovimiento)
                .AsNoTracking()
                .ToListAsync();

            // Mapeo manual si no tienes mapper aún:
            return movimientos.Select(m => new Dom.MovimientoStock
            {
                IdMovimientoStock = m.IdMovimientoStock,
                Cantidad = m.Cantidad,
                FechaMovimiento = m.FechaMovimiento,
                // Mapea Producto, FilaOrigen, FilaDestino, Usuario según tus mappers
            });
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