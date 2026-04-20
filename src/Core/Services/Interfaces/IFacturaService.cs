using Core.Common;
using src.Core.Contracts;
using Dom = src.Models.Domain;

namespace src.Core.Services.Interfaces;

public interface IFacturaService
{
    Task<IEnumerable<Dom.Factura>> ObtenerTodasAsync();
    Task<ServiceResult<Dom.Factura>> ObtenerPorIdAsync(int id);
    Task<ServiceResult<DocumentosRelacionadosData>> ObtenerDocumentosAsociadosAsync(int idFactura);
    Task<ServiceResult<Dom.Compra>> ValidarCompraParaFacturacionAsync(int idCompra);
    Task<IEnumerable<Dom.Factura>> ObtenerPendientesPagoAsync();
    Task<bool> ExisteNumeroFacturaAsync(short idProveedor, string numero);
    Task<ServiceResult<int>> CrearDesdeCompraAsync(Dom.Factura factura, int idCompra);
    Task<ServiceResult> ActualizarAsync(Dom.Factura factura);
}
