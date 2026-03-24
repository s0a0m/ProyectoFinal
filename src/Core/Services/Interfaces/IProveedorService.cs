using src.Core.Contracts;
using src.ViewModels;
using Dom = src.Models.Domain;

namespace src.Core.Services.Interfaces;

public interface IProveedorService
{
    Task<Dom.Proveedor> CreateProveedorAsync(CrearProveedorViewModel proveedorVM);
    Task<Dom.Proveedor> GetProveedorByIdAsync(int IdProveedor);
    Task<IEnumerable<Dom.Proveedor>> GetActiveProveedoresAsync();
    Task UpdateProveedorAsync(ActualizarProveedorViewModel proveedorVM);
    Task DeleteAsync(int id);
    Task ReactivateAsync(int id);
    Task<CuentaCorrienteData?> ObtenerCuentaCorrienteAsync(short idProveedor);
}

