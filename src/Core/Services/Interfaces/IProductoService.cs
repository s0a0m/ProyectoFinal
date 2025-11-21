using src.Presentation.ViewModels.ProductoVM;
using Dom = src.Models.Domain;

namespace src.Core.Services.Interfaces
{
    public interface IProductoService
    {
        Task<IEnumerable<ProductoListarViewModel>> GetAllParaListadoAsync();
        Task<IEnumerable<ProductoListarViewModel>> GetProductosBajoStockAsync(); // RF 2.8
        
        Task<ActualizarProductoViewModel> PrepararActualizarViewModelAsync(int id);
        Task<CrearProductoViewModel> PrepararCrearViewModelAsync();
        
        Task CreateAsync(CrearProductoViewModel vm);
        Task UpdateAsync(ActualizarProductoViewModel vm);
        Task DeleteAsync(int id);
        
        // Método auxiliar para DataTables si necesitas recargar el VM en error
        Task RepoblarViewModelAsync(CrearProductoViewModel vm);
        Task RepoblarViewModelAsync(ActualizarProductoViewModel vm);
        Task ReactivateAsync(int id);
    }
}