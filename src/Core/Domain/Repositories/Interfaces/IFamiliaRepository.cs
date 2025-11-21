using System.Collections.Generic;
using System.Threading.Tasks;
using Dom = src.Models.Domain;

namespace src.Repositories.Interfaces
{
    public interface IFamiliaRepository
    {
        Task<IEnumerable<Dom.Familia>> GetAllAsync();
        
        //Trae familias Y sus categorías
        Task<IEnumerable<Dom.Familia>> GetAllWithCategoriasAsync();
        
        Task<Dom.Familia?> GetByIdAsync(short id);
        Task AddAsync(Dom.Familia entity);
        Task UpdateAsync(Dom.Familia entity);
        
        // Validación antes de borrar
        Task<bool> HasCategoriasAsync(short idFamilia);
        Task DeleteAsync(short id);

        Task<bool> ExistsNombreAsync(string nombre, short? idExcluir = null);
    }
}