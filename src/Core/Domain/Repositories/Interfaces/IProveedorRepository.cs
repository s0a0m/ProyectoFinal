using Dom = src.Models.Domain;
namespace src.Repositories.Interfaces;

public interface IProveedorRepository
{
    Task<IEnumerable<Dom.Proveedor>> GetAllProveedorAsync();
    Task<Dom.Proveedor?> GetProveedorById(int id);
    Task AddAsync(Dom.Proveedor entity);
    Task UpdateAsync(Dom.Proveedor entity);
    Task<bool> DeleteAsync(int id);
    Task<bool> ReactivateAsync(int id);
    Task ActualizarSaldoAsync(int idProveedor, decimal nuevoSaldo);
}
