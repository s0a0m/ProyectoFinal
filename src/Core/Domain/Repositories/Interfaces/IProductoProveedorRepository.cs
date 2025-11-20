using Dom = src.Models.Domain;

namespace src.Repositories.Interfaces;

public interface IProductoProveedorRepository
{
    Task<IEnumerable<Dom.ProductoProveedor>> GetAllAsync();
    
    Task<Dom.ProductoProveedor?> GetByIdAsync(int idProducto, int idProveedor);

    //  Ver todos los productos de un proveedor específico 
    Task<IEnumerable<Dom.ProductoProveedor>> GetByProveedorIdAsync(int idProveedor);
    Task AddAsync(Dom.ProductoProveedor entity);
    Task UpdateAsync(Dom.ProductoProveedor entity);
    Task DeleteAsync(int idProducto, int idProveedor);
    // Validación rápida
    Task<bool> ExistsAsync(int idProducto, int idProveedor);
}