using Dom = src.Models.Domain;
using src.Contracts;

namespace src.Repositories.Interfaces;

public interface IProductoProveedorRepository
{
    Task<IEnumerable<ProductoProveedorDto>> GetAllAsync();

    Task<ProductoProveedorDto?> GetByIdAsync(int idProducto, int idProveedor);

    //  Ver todos los productos de un proveedor específico 
    Task<IEnumerable<Dom.ProductoProveedor>> GetByProveedorIdAsync(int idProveedor);
    Task AddAsync(Dom.ProductoProveedor entity, List<string> codigosExternos);
    Task UpdateAsync(Dom.ProductoProveedor entity, CancellationToken cancellationToken = default);
    Task DesactivarPorProductoAsync(int idProducto);
    Task DesactivarPorProveedorAsync(int idProveedor);
    Task ReactivarPorProductoAsync(int idProducto);

    Task ReactivarPorProveedorAsync(int idProveedor);
    Task<bool> ExistsAsync(int idProducto, int idProveedor);
}