using src.ViewModels;
using Dom = src.Models.Domain;
using src.Contracts;
using src.Presentation.ViewModels.ProductoVM;
using Core.Common;
namespace src.Core.Services.Interfaces;

public interface IProductoProveedorService
{
    Task<CrearProductoProveedorViewModel> PrepararCrearViewModelAsync();
    Task<IEnumerable<ProductoProveedorListarViewModel>> GetAllParaListadoAsync();
    Task<ActualizarProductoProveedorViewModel> PrepararActualizarViewModelAsync(int idProducto, int idProveedor);
    Task<ServiceResult> CreateAsync(CrearProductoProveedorViewModel vm);
    Task<ServiceResult> UpdateAsync(ActualizarProductoProveedorViewModel vm);
    Task RepoblarViewModelAsync(CrearProductoProveedorViewModel vm);
    Task GestionarCascadaProductoAsync(int idProducto, bool activando);
    Task GestionarCascadaProveedorAsync(int idProveedor, bool activando);

}