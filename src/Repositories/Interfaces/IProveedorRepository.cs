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


// // Repositories/IProveedorRepository.cs
// using src.Models;

// namespace src.Repositories.Interfaces;

// public interface IProveedorRepository
// {
//     Task<ProveedorDto?> GetProvByIdAsync(int id);
//     Task<List<ProveedorDto>> GetAllProvAsync();
//     Task<ProveedorDto> CreateAsync(ProveedorDto proveedor);
//     Task<ProveedorDto> UpdateAsync(ProveedorDto proveedor);
//     Task<bool> DeleteAsync(int id);
//     Task<List<Provincia>> GetAllProvinciasAsync();
// }