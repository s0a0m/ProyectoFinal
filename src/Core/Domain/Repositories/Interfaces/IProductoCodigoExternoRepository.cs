using Dom = src.Models.Domain;
namespace src.Repositories.Interfaces;

public interface IProductoCodigoExternoRepository
{
    Task<Dom.Producto?> ObtenerProductoPorCodigoAsync(string codigoExterno, short idProveedor);
    Task<bool> ExistsAsync(string codigo);
}
