using System.Collections.Generic;
using System.Threading.Tasks;
using Dom = src.Models.Domain;

namespace src.Repositories.Interfaces
{
    public interface ICategoriaRepository
    {
        Task<IEnumerable<Dom.Categoria>> GetAllAsync();
        
        // METODO UTIL: Filtrar categorías de una familia específica
        Task<IEnumerable<Dom.Categoria>> GetByFamiliaIdAsync(short idFamilia);
        
        Task<Dom.Categoria?> GetByIdAsync(short id);
        Task AddAsync(Dom.Categoria entity);
        Task UpdateAsync(Dom.Categoria entity);
        
        // Validación útil: no borrar si tiene productos
        Task<bool> HasProductosAsync(short idCategoria);
        Task DeleteAsync(short id);
        Task<bool> ExistsNombreEnFamiliaAsync(string nombre, short idFamilia, short? idExcluir = null);
    }
}