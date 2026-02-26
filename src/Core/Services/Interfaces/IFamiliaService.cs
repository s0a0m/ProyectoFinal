using System.Collections.Generic;
using System.Threading.Tasks;
using Dom = src.Models.Domain;
using src.Presentation.ViewModels.FamiliaVM; 

namespace src.Core.Services.Interfaces
{
    public interface IFamiliaService
    {
        Task<IEnumerable<Dom.Familia>> GetAllAsync();
        Task<IEnumerable<Dom.Familia>> GetAllWithCategoriasAsync(); 
        Task<Dom.Familia> GetByIdAsync(short id);
        Task<Dom.Familia> CreateAsync(CrearFamiliaViewModel vm);
        Task UpdateAsync(ActualizarFamiliaViewModel vm);
        Task DeleteAsync(short id);
    }
}