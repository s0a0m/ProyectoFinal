using Core.Common;
using src.Core.Contracts;
using Dom = src.Models.Domain;

namespace src.Core.Services.Interfaces;

public interface IComprobanteService
{
    Task<ServiceResult<IEnumerable<Dom.Comprobante>>> ObtenerPorFacturaAsync(int facturaId);
    Task<ServiceResult<Dom.Comprobante>> ObtenerDetalleNotaAsync(int id);
    Task<Dom.Comprobante?> ObtenerPorIdAsync(int id);
    Task<ServiceResult<int>> CrearComprobanteAsync(Dom.Comprobante comprobante);
    Task<ServiceResult<GestionNotasData>> ObtenerGestionNotasAsync(int idProveedor);
}
