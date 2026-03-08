using Dom = src.Models.Domain;
public interface IUbicacionProductoRepository
{
    Task<IEnumerable<Dom.UbicacionProducto>> GetByProductoAsync(int idProducto);
    Task<IEnumerable<Dom.UbicacionProducto>> GetByFilaAsync(int idFila);
    Task AgregarStockEnFilaAsync(int idProducto, int idFila, decimal cantidad, int idUsuario);
    Task MoverStockAsync(int idProducto, int idFilaOrigen, int idFilaDestino, decimal cantidad, int idUsuario);
    Task RetirarStockDeFilaAsync(int idProducto, int idFila, decimal cantidad, int idUsuario);
 
    Task<IEnumerable<Dom.MovimientoStock>> GetMovimientosAsync(int? idProducto = null, int? idDeposito = null);
}