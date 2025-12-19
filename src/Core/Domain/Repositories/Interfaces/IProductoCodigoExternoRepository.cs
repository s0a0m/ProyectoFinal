using Dom = src.Models.Domain;
namespace src.Repositories.Interfaces;

public interface IProductoCodigoExternoRepository
{
    Task<Dom.Producto?> ObtenerProductoPorCodigoAsync(string codigoExterno, short idProveedor, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string codigo, short id_proveedor);
    Task AddAsync(short idProducto, short idProveedor, string codigo);
}
