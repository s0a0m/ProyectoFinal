using Microsoft.AspNetCore.Mvc.Rendering;
using src.Presentation.ViewModels.StockVM;
using src.Repositories.Interfaces;
using src.Core.Services.Interfaces;

namespace src.Services.Implementations;

public class StockService : IStockService
{
    private readonly IUbicacionProductoRepository _ubicacionRepo;
    private readonly IProductoRepository          _productoRepo;
    private readonly IFilaRepository              _filaRepo;
    private readonly IDepositoRepository          _depositoRepo;

    public StockService(
        IUbicacionProductoRepository ubicacionRepo,
        IProductoRepository          productoRepo,
        IFilaRepository              filaRepo,
        IDepositoRepository          depositoRepo)
    {
        _ubicacionRepo = ubicacionRepo;
        _productoRepo  = productoRepo;
        _filaRepo      = filaRepo;
        _depositoRepo  = depositoRepo;
    }

    // ════════════════════════════════════════════════════════════════════════
    //  LEGACY — sin cambios respecto al código original
    // ════════════════════════════════════════════════════════════════════════

    // public async Task<StockPorProductoViewModel> BuscarProductoStockAsync(string termino)
    // {
    //     var vm = new StockPorProductoViewModel { TerminoBusqueda = termino };
    //     if (string.IsNullOrWhiteSpace(termino)) return vm;

    //     try
    //     {
    //         var producto = await _productoRepo.BuscarPorNombreOCodigoAsync(termino.Trim());
    //         if (producto is null)
    //         {
    //             vm.NombreProducto = "No se encontró ningún producto con ese nombre o código.";
    //             return vm;
    //         }

    //         vm.IdProducto     = producto.IdProducto;
    //         vm.NombreProducto = producto.Nombre;
    //         vm.StockTotal     = producto.StockTotal;

    //         var ubicaciones = await _ubicacionRepo.GetByProductoAsync(producto.IdProducto);
    //         vm.Ubicaciones = ubicaciones.Select(MapUbicacion).ToList();

    //         var todasFilas = await _filaRepo.GetAllAsync();
    //         vm.FilasDestino = todasFilas
    //             .Where(f => f.Activo)
    //             .Select(f => new SelectListItem
    //             {
    //                 Value = f.IdFila.ToString(),
    //                 Text  = $"{f.Estante.Deposito.Nombre}  ▸  {f.Estante.NumeroEstante}  ▸  {f.NFila}"
    //             });
    //     }
    //     catch (Exception ex)
    //     {
    //         throw new ApplicationException(
    //             "Error al buscar el producto o su distribución de stock.", ex);
    //     }

    //     return vm;
    // }

    public async Task<MoverStockViewModel?> GetMoverStockFormAsync(int idProducto, int idFilaOrigen)
    {
        try
        {
            var producto = await _productoRepo.GetByIdAsync(idProducto);
            if (producto is null) return null;

            var ubicaciones = await _ubicacionRepo.GetByProductoAsync(idProducto);
            var todasFilas  = await _filaRepo.GetAllAsync();

            return new MoverStockViewModel
            {
                IdProducto          = idProducto,
                NombreProducto      = producto.Nombre,
                IdFilaOrigen        = idFilaOrigen,
                UbicacionesActuales = ubicaciones.Select(MapUbicacion).ToList(),
                FilasDestino = todasFilas
                    .Where(f => f.Activo && f.IdFila != idFilaOrigen)
                    .Select(f => new SelectListItem
                    {
                        Value = f.IdFila.ToString(),
                        Text  = $"{f.Estante.Deposito.Nombre}  ▸  Estante {f.Estante.NumeroEstante}  ▸  Fila {f.NFila}"
                    })
            };
        }
        catch (Exception ex)
        {
            throw new ApplicationException(
                "Error al preparar el formulario de movimiento de stock.", ex);
        }
    }

    public async Task<(bool success, string message)> MoverStockAsync(
    MoverStockViewModel vm, int idUsuario)
    {
        try
        {
            if (vm.IdFilaOrigen == vm.IdFilaDestino)
                return (false, "La fila de origen y destino no pueden ser la misma.");
            if (vm.Cantidad <= 0)
                return (false, "La cantidad a mover debe ser mayor a cero.");

            await _ubicacionRepo.MoverStockAsync(
                vm.IdProducto, vm.IdFilaOrigen, vm.IdFilaDestino, vm.Cantidad, idUsuario);

            return (true,
                $"Se movieron {vm.Cantidad:N2} unidades de \"{vm.NombreProducto}\" correctamente.");
        }
        catch (InvalidOperationException ex) { return (false, ex.Message); }
        catch (ArgumentException         ex) { return (false, ex.Message); }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
        {
            // Expone el error real de BD (constraint, campo nulo, etc.)
            var inner = ex.InnerException?.Message ?? ex.Message;
            return (false, $"Error de base de datos al mover stock: {inner}");
        }
        catch (Exception ex)
        {
            // Expone tipo y mensaje real para debugging
            var inner = ex.InnerException?.Message ?? "";
            return (false,
                $"[{ex.GetType().Name}] {ex.Message}" +
                (string.IsNullOrEmpty(inner) ? "" : $" | Inner: {inner}"));
        }
    }

    public async Task<AdministrarStockDepositoViewModel?> GetStockDepositoAsync(int idDeposito)
    {
        try
        {
            var deposito = await _depositoRepo.GetByIdAsync(idDeposito);
            if (deposito is null) return null;

            var vm = new AdministrarStockDepositoViewModel
            {
                IdDeposito     = idDeposito,
                NombreDeposito = deposito.Nombre
            };

            var productosDict = new Dictionary<int, ProductoEnDepositoViewModel>();

            foreach (var estante in deposito.Estantes ?? [])
            foreach (var fila    in estante.Filas    ?? [])
            {
                var ubicaciones = await _ubicacionRepo.GetByFilaAsync(fila.IdFila);
                foreach (var ub in ubicaciones)
                {
                    if (!productosDict.TryGetValue(ub.Producto.IdProducto, out var prodVm))
                    {
                        prodVm = new ProductoEnDepositoViewModel
                        {
                            IdProducto     = ub.Producto.IdProducto,
                            NombreProducto = ub.Producto.Nombre,
                            Ubicaciones    = new List<UbicacionStockItemViewModel>()
                        };
                        productosDict[ub.Producto.IdProducto] = prodVm;
                    }

                    ((List<UbicacionStockItemViewModel>)prodVm.Ubicaciones).Add(MapUbicacion(ub));
                    prodVm.StockEnDeposito += ub.Cantidad;
                }
            }

            vm.Productos = productosDict.Values.OrderBy(p => p.NombreProducto);
            return vm;
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Error al obtener el stock del depósito.", ex);
        }
    }

    public async Task<(bool success, string message)> AgregarStockAsync(
        int idProducto, int idFila, decimal cantidad, int idUsuario)
    {
        try
        {
            if (cantidad <= 0)
                return (false, "La cantidad a agregar debe ser mayor a cero.");

            await _ubicacionRepo.AgregarStockEnFilaAsync(idProducto, idFila, cantidad, idUsuario);
            return (true, $"Se agregaron {cantidad:N2} unidades al stock correctamente.");
        }
        catch (InvalidOperationException ex) { return (false, ex.Message); }
        catch (KeyNotFoundException       ex) { return (false, ex.Message); }
        catch (ArgumentException          ex) { return (false, ex.Message); }
        catch (Exception)  { return (false, "Error inesperado al agregar el stock."); }
    }

    public async Task<(bool success, string message)> RetirarStockAsync(
        int idProducto, int idFila, decimal cantidad, int idUsuario)
    {
        try
        {
            if (cantidad <= 0)
                return (false, "La cantidad a retirar debe ser mayor a cero.");

            await _ubicacionRepo.RetirarStockDeFilaAsync(idProducto, idFila, cantidad, idUsuario);
            return (true, $"Se retiraron {cantidad:N2} unidades del stock correctamente.");
        }
        catch (InvalidOperationException ex) { return (false, ex.Message); }
        catch (KeyNotFoundException       ex) { return (false, ex.Message); }
        catch (ArgumentException          ex) { return (false, ex.Message); }
        catch (Exception)  { return (false, "Error inesperado al retirar el stock."); }
    }

    public async Task<IEnumerable<MovimientoStockItemViewModel>> GetHistorialAsync(
    int? idProducto = null, int? idDeposito = null)
    {
        try
        {
            var movimientos = await _ubicacionRepo.GetMovimientosAsync(idProducto, idDeposito);
            return movimientos.Select(m => new MovimientoStockItemViewModel
            {
                IdMovimientoStock = m.IdMovimientoStock,
                NombreProducto    = m.Producto?.Nombre   ?? string.Empty,
                FilaOrigen        = FormatearRuta(m.FilaOrigen),
                FilaDestino       = FormatearRuta(m.FilaDestino),
                Cantidad          = m.Cantidad,
                FechaMovimiento   = m.FechaMovimiento,
                Usuario           = m.Usuario?.Nombre    ?? "—",
                EsIngreso         = m.FilaOrigen  is null,   // ← dato real, no string
                EsEgreso          = m.FilaDestino is null,
            });
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Error al obtener el historial de movimientos.", ex);
        }
    }
    // ════════════════════════════════════════════════════════════════════════
    //  NUEVOS — para BuscarProducto
    // ════════════════════════════════════════════════════════════════════════

    /// Retorna el detalle de stock completo de un producto por su ID.
    public async Task<ProductoStockDetalleViewModel?> GetStockProductoAsync(int idProducto)
    {
        try
        {
            var producto = await _productoRepo.GetByIdAsync(idProducto);
            if (producto is null) return null;

            var ubicaciones = await _ubicacionRepo.GetByProductoAsync(idProducto);

            return new ProductoStockDetalleViewModel
            {
                IdProducto     = producto.IdProducto,
                NombreProducto = producto.Nombre,
                CodigosBarras  = producto.CodigoBarra?
                                    .Select(c => c.Codigo)
                                    .ToList() ?? [],
                StockTotal     = ubicaciones.Sum(u => u.Cantidad),
                Ubicaciones    = ubicaciones.Select(MapUbicacion).ToList()
            };
        }
        catch (Exception ex)
        {
            throw new ApplicationException(
                "Error al obtener el detalle de stock del producto.", ex);
        }
    }

    /// Retorna el detalle de stock de un producto buscando por código de barras exacto.
    public async Task<ProductoStockDetalleViewModel?> BuscarPorCodigoAsync(string codigo)
    {
        try
        {
            // Antes llamaba a BuscarPorNombreOCodigoAsync que hacía fallback a nombre.
            // Ahora solo busca por código exacto.
            var producto = await _productoRepo.BuscarPorCodigoExactoAsync(codigo.Trim());
            if (producto is null) return null;

            return await GetStockProductoAsync(producto.IdProducto);
        }
        catch (Exception ex)
        {
            throw new ApplicationException(
                "Error al buscar el producto por código de barras.", ex);
        }
    }

    public async Task<List<ProductoBusquedaItemViewModel>> BuscarPorNombreAsync(string termino)
    {
        try
        {
            var productos = await _productoRepo.BuscarListaPorNombreAsync(termino.Trim());
            // Ya no puede ser null ni contener nulls — retorna IEnumerable<Dom.Producto>

            var resultado = new List<ProductoBusquedaItemViewModel>();
            foreach (var p in productos)
            {
                var ubicaciones = await _ubicacionRepo.GetByProductoAsync(p.IdProducto);
                resultado.Add(new ProductoBusquedaItemViewModel
                {
                    IdProducto    = p.IdProducto,
                    Nombre        = p.Nombre,
                    CodigosBarras = string.Join(", ",
                        p.CodigoBarra?.Select(c => c.Codigo) ?? []),
                    StockTotal    = ubicaciones.Sum(u => u.Cantidad),
                    CantidadFilas = ubicaciones.Count()
                });
            }
            return resultado;
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Error al buscar productos por nombre.", ex);
        }
    }

    // ════════════════════════════════════════════════════════════════════════
    //  HELPERS PRIVADOS
    // ════════════════════════════════════════════════════════════════════════

    private static UbicacionStockItemViewModel MapUbicacion(
    Models.Domain.UbicacionProducto up) => new()
    {
        IdUbicacion    = 0,   // ← UbicacionProducto no tiene PK propia
        IdFila         = up.Fila.IdFila,
        NFila          = up.Fila.NFila,
        IdEstante      = up.Fila.Estante.IdEstante,
        NumeroEstante  = up.Fila.Estante.NumeroEstante,
        IdDeposito     = up.Fila.Estante.Deposito.IdDeposito,
        NombreDeposito = up.Fila.Estante.Deposito.Nombre,
        Cantidad       = up.Cantidad
    };

    private static string FormatearRuta(Models.Domain.Fila? fila)
    {
        if (fila is null) return "—";
        return $"{fila.Estante?.Deposito?.Nombre}  ▸  " +
               $"Estante {fila.Estante?.NumeroEstante}  ▸  " +
               $"Fila {fila.NFila}";
    }
}