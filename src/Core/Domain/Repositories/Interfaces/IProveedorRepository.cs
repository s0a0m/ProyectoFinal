// /* using src.Models;
// using src.Models.CodeFirst;

// namespace src.Repositories.Interfaces
// {
//     public interface IProveedorRepository
//     {
//         Task<List<proveedor>> GetAll();
//         Task<proveedor>? GetById(int id);
//         Task Add(proveedor proveedor);
//         Task Update(proveedor proveedor);
//         Task Delete(int id);
//     }
// }*/


// Repositories/IProveedorRepository.cs
using EF = src.Models.CodeFirst;
using Dom = src.Models.Domain;
namespace src.Repositories.Interfaces;

public interface IProveedorRepository
{
    Task<IEnumerable<Dom.Proveedor>> GetAllProveedorAsync();
    Task<Dom.Proveedor?> GetProveedorById(int id);
    Task AddAsync(Dom.Proveedor entity);
    Task UpdateAsync(Dom.Proveedor entity);

    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<Dom.Provincia>> GetAllProvinciaAsync();
   /* Task<Proveedor?> GetProvByIdAsync(int id);
    Task<List<Proveedor>> GetAllProvAsync();
    Task<Proveedor> CreateAsync(Proveedor proveedor);
    Task<Proveedor> UpdateAsync(Proveedor proveedor);
    Task<bool> DeleteAsync(int id);
    Task<List<Provincia>> GetAllProvinciasAsync(); */
}
