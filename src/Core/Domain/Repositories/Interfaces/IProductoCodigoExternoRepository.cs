using Dom = src.Models.Domain;
namespace src.Repositories.Interfaces;

public interface IProductoCodigoExternoRepository
{
    Task<Dom.Producto?> ObtenerProductoPorCodigoAsync(string codigoExterno, short idProveedor);
    // Task AddAsync(Dom.ProductoCodigoExterno entity);
    // Task UpdateAsync(Dom.ProductoCodigoExterno entity);
    // Task DeleteAsync(int idProducto, int idProveedor);
    // Task<Dom.ProductoCodigoExterno?> GetByIdAsync(int idProducto, int idProveedor);
}
