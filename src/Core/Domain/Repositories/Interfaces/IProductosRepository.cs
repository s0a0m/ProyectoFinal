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
        Task ReactivateAsync(int id);

        /// Busca un producto por nombre (LIKE) o por código de barra exacto.

        // Métodos de soporte para validaciones de negocio (RF 2.3.1)
        Task<bool> ExistsCodigoBarraAsync(string codigo, int? excluirProductoId = null);
        Task<IEnumerable<Dom.Producto>> GetBajosDeStockAsync();

        // Cambiar estas dos firmas en la interfaz:
        Task<IEnumerable<Dom.Producto>> BuscarListaPorNombreAsync(string termino);
        Task<Dom.Producto?>              BuscarPorCodigoExactoAsync(string codigo);
    }
}