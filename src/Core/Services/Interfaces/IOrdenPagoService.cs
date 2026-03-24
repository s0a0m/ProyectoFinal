using Core.Common;
using Dom = src.Models.Domain;

namespace src.Core.Services.Interfaces;

public interface IOrdenPagoService
{
    Task<Dom.OrdenPago?> ObtenerPorIdAsync(int idOrden);
    Task<IEnumerable<Dom.OrdenPago>> ObtenerPorProveedorAsync(short idProveedor);
    Task<IEnumerable<Dom.Factura>> ObtenerFacturasPendientesAsync(short idProveedor);
    Task<ServiceResult<int>> CrearOrdenAsync(Dom.OrdenPago orden);
    Task<ServiceResult> ConfirmarOrdenAsync(int idOrden);
    Task<ServiceResult> EliminarOrdenAsync(int idOrden);
}

