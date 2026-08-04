using System.Collections.Generic;
using System.Threading.Tasks;
using Dom = src.Models.Domain;
using src.Presentation.ViewModels.CategoriaVM; // Asumo este namespace

namespace src.Core.Services.Interfaces
{
    public interface ICategoriaService
    {
        Task<IEnumerable<Dom.Categoria>> GetAllAsync();
        Task<IEnumerable<Dom.Categoria>> GetByFamiliaIdAsync(short idFamilia);
        Task<Dom.Categoria> GetByIdAsync(short id);
        Task<Dom.Categoria> CreateAsync(CrearCategoriaViewModel vm);
        Task UpdateAsync(ActualizarCategoriaViewModel vm);
        Task DeleteAsync(short id);
    }
}