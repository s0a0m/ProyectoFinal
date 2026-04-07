using Core.Common;
using src.Core.Contracts;
using Dom = src.Models.Domain;

namespace src.Core.Services.Interfaces;

public interface IFacturaService
{
    Task<IEnumerable<Dom.Factura>> ObtenerTodasAsync();
    Task<Dom.Factura?> ObtenerPorIdAsync(int id);
    Task<IEnumerable<Dom.Factura>> ObtenerPendientesPagoAsync();
    Task<bool> ExisteNumeroFacturaAsync(short idProveedor, string numero);
    Task<DocumentosRelacionadosData?> ObtenerDocumentosAsociadosAsync(int idFactura);
    Task<ServiceResult<int>> CrearDesdeCompraAsync(Dom.Factura factura, int idCompra);
    Task<ServiceResult> ActualizarAsync(Dom.Factura factura);
}
