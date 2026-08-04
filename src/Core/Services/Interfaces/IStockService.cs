using src.Presentation.ViewModels.StockVM;

namespace src.Core.Services.Interfaces;

public interface IStockService
{
    // ── Métodos legacy (StockService ya los implementa) ─────────────────────
    Task<MoverStockViewModel?>                  GetMoverStockFormAsync(int idProducto, int idFilaOrigen);
    Task<(bool success, string message)>        MoverStockAsync(MoverStockViewModel vm, int idUsuario);
    Task<AdministrarStockDepositoViewModel?>    GetStockDepositoAsync(int idDeposito);
    Task<(bool success, string message)>        AgregarStockAsync(int idProducto, int idFila, decimal cantidad, int idUsuario);
    Task<(bool success, string message)>        RetirarStockAsync(int idProducto, int idFila, decimal cantidad, int idUsuario);

    // ── Nuevos métodos para BuscarProducto ───────────────────────────────────
    Task<ProductoStockDetalleViewModel?>        GetStockProductoAsync(int idProducto);
    Task<ProductoStockDetalleViewModel?>        BuscarPorCodigoAsync(string codigo);
    Task<List<ProductoBusquedaItemViewModel>>   BuscarPorNombreAsync(string termino);

    Task<IEnumerable<MovimientoStockItemViewModel>> GetHistorialAsync(int? idProducto = null, int? idDeposito = null);

}