using src.ViewModels;
using Dom = src.Models.Domain;
using src.Contracts;
using src.Presentation.ViewModels.ProductoVM;
namespace src.Core.Services.Interfaces;

public interface IProductoProveedorService
{
    Task<IEnumerable<AccionDeFilaCargaAutomatica>> ProcesarListaDePreciosAsync(Stream fileStream, short idProveedor, ImportacionColumnaMap mapaColumnas);
    Task<CrearProductoProveedorViewModel> PrepararCrearViewModelAsync();
    Task<IEnumerable<ProductoProveedorListarViewModel>> GetAllParaListadoAsync();
    Task<ActualizarProductoProveedorViewModel> PrepararActualizarViewModelAsync(int idProducto, int idProveedor);
    Task CreateAsync(CrearProductoProveedorViewModel vm);
    Task UpdateAsync(ActualizarProductoProveedorViewModel vm);      
    Task RepoblarViewModelAsync(CrearProductoProveedorViewModel vm);
    Task GestionarCascadaProductoAsync(int idProducto, bool activando);
    Task GestionarCascadaProveedorAsync(int idProveedor, bool activando);
}