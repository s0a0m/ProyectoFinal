using Core.Common;
using src.Core.Contracts;
using Dom = src.Models.Domain;

namespace src.Core.Services.Interfaces;

public interface IOrdenPagoService
{
    Task<ServiceResult<Dom.OrdenPago>> ObtenerDetalleOrdenAsync(int idOrden);
    Task<ServiceResult<HistorialPagosData>> ObtenerHistorialPagosAsync(short idProveedor);
    Task<ServiceResult<int>> CrearOrdenAsync(Dom.OrdenPago orden);
    Task<ServiceResult<NuevoPagoData>> ObtenerDatosParaNuevoPagoAsync(short idProveedor);
    Task<ServiceResult> ConfirmarOrdenAsync(int idOrden);
    Task<ServiceResult> EliminarOrdenAsync(int idOrden);
}
