// using src.Presentation.ViewModels.CuentaCorrienteVM;
//
// namespace src.Core.Services.Interfaces
// {
//     public interface IDocumentoAsociadoService
//     {
//         Task<OrdenPagoDetalleModalVM?> ObtenerDetalleParaModalAsync(int idOrden);
//         Task<ModuloOrdenesPagoVM> ObtenerModuloPorProveedorAsync(short idProveedor);
//         Task<OrdenPagoFormVM> ObtenerFormularioCreacionAsync(short idProveedor);
//
//         Task CrearOrdenAsync(OrdenPagoFormVM vm);
//         Task EliminarOrdenAsync(int idOrden);
//          Task ConfirmarOrdenAsync(int idOrden);
//     }
// }

using Core.Common;
using src.Core.Contracts;
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

