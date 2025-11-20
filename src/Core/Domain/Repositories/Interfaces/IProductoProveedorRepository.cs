using Dom = src.Models.Domain;
namespace src.Repositories.Interfaces;

public interface IProductoProveedorRepository
{
    Task UpdateAsync(Dom.ProductoProveedor entity);
}
