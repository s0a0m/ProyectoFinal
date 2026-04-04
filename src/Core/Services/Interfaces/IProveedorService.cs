using src.Core.Contracts;
using src.Presentation.ViewModels.ProveedorVM;
using src.ViewModels;
using Dom = src.Models.Domain;

namespace src.Core.Services.Interfaces;

public interface IProveedorService
{
    Task<Dom.Proveedor> CreateProveedorAsync(Dom.Proveedor proveedorVM);
    Task<Dom.Proveedor> GetProveedorByIdAsync(int IdProveedor);
    Task<DetalleProveedorViewModel> GetDetalleProveedorByIdAsync(int IdProveedor);
    Task<IEnumerable<ListarProveedorViewModel>> GetActiveProveedoresAsync();
    Task UpdateProveedorAsync(ActualizarProveedorViewModel proveedorVM);
    Task DeleteAsync(int id);
    Task ReactivateAsync(int id);
    Task<CuentaCorrienteData?> ObtenerCuentaCorrienteAsync(short idProveedor);
}
