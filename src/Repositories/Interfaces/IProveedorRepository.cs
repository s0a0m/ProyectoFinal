/* using src.Models;
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
}*/


// Repositories/IProveedorRepository.cs
using src.Models;

namespace src.Repositories
{
    public interface IProveedorRepository
    {
        Task<Proveedor?> GetProvByIdAsync(int id);
        Task<List<Proveedor>> GetAllProvAsync();
        Task<Proveedor> CreateAsync(Proveedor proveedor);
        Task<Proveedor> UpdateAsync(Proveedor proveedor);
        Task<bool> DeleteAsync(int id);
    }
}