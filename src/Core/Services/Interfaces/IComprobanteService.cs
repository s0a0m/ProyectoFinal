using Core.Common;
using src.Core.Contracts;
using Dom = src.Models.Domain;

namespace src.Core.Services.Interfaces;

public interface IComprobanteService
{
    Task<IEnumerable<Dom.Comprobante>> ObtenerPorFacturaAsync(int facturaId);
    Task<Dom.Comprobante?> ObtenerPorIdAsync(int id);
    Task<ServiceResult<int>> CrearComprobanteAsync(Dom.Comprobante comprobante);
    Task<GestionNotasData?> ObtenerGestionNotasAsync(int idProveedor);
}
