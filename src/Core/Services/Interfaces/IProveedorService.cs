using src.ViewModels;
using src.Presentation.ViewModels.CuentaCorrienteVM;
using Dom = src.Models.Domain;
using src.Contracts;
namespace src.Core.Services.Interfaces;

public interface IProveedorService
{
    Task<Dom.Proveedor> CreateProveedorAsync(CrearProveedorViewModel proveedorVM);
    Task<Dom.Proveedor> GetProveedorByIdAsync(int IdProveedor);
    Task<IEnumerable<Dom.Proveedor>> GetActiveProveedoresAsync();
    Task UpdateProveedorAsync(ActualizarProveedorViewModel proveedorVM);
    Task DeleteAsync(int id);      
    Task ReactivateAsync(int id);
    Task<CuentaCorrienteVM> ObtenerCuentaCorrienteAsync(short idProveedor);
}