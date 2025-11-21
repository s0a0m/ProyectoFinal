using Dom = src.Models.Domain;
using src.Contracts;

namespace src.Repositories.Interfaces;

public interface IProductoProveedorRepository
{
    Task<IEnumerable<ProductoProveedorDto>> GetAllAsync();
    
    Task<ProductoProveedorDto?> GetByIdAsync(int idProducto, int idProveedor);

    //  Ver todos los productos de un proveedor específico 
    Task<IEnumerable<Dom.ProductoProveedor>> GetByProveedorIdAsync(int idProveedor);
    Task AddAsync(Dom.ProductoProveedor entity, string codigoExterno);
    Task UpdateAsync(Dom.ProductoProveedor entity);
    Task DeleteAsync(int idProducto, int idProveedor);
    // Validación rápida
    Task<bool> ExistsAsync(int idProducto, int idProveedor);
}