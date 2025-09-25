using src.Models;
using src.Models.CodeFirst;

namespace src.Repositories.Interfaces
{
    public interface IProveedorRepository
    {
        Task<List<proveedor>> GetAll();
        Task<proveedor>? GetById(int id);
        Task Add(proveedor proveedor);
        Task Update(proveedor proveedor);
        Task Delete(int id);
    }
}