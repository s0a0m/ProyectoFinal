using System.Collections.Generic;
using System.Threading.Tasks;
using Dom = src.Models.Domain;

namespace src.Repositories.Interfaces
{
    public interface IProductoRepository
    {
        // CRUD Básico
        Task<IEnumerable<Dom.Producto>> GetAllAsync(); 
        Task<Dom.Producto?> GetByIdAsync(int id);
        Task AddAsync(Dom.Producto entity);
        Task UpdateAsync(Dom.Producto entity);
        Task DeleteAsync(int id);

        // Métodos de soporte para validaciones de negocio (RF 2.3.1)
        Task<bool> ExistsCodigoBarraAsync(string codigo, int? excluirProductoId = null);
        
    }
}