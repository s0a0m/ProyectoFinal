using src.Presentation.ViewModels.CompraVM;
using src.Presentation.ViewModels.FacturaVM;
using Dom = src.Models.Domain;

namespace src.Core.Services.Interfaces
{
    public interface IFacturaService
    {
        Task<IEnumerable<ListarFacturaViewModel>> ObtenerTodasAsync();
        Task<ListarDetalleFacturaViewModel?> ObtenerPorIdAsync(int id);
        Task<IEnumerable<ListarFacturaViewModel>> ObtenerPendientesPagoAsync();
        Task<int> CrearFacturaAsync(CrearFacturaViewModel modelo);
        Task<bool> ActualizarEstadoPagoAsync(int id, bool pagada);
        Task<bool> ExisteNumeroFacturaAsync(short idProveedor, string numero);
        // IFacturaService.cs
        Task<CrearFacturaViewModel> PrepararFacturaDesdeCompraAsync(int idCompra);
        Task<DocumentosRelacionadosViewModel> ObtenerDocumentosAsociadosAsync(int idFactura);
        Task<ActualizarFacturaViewModel> PrepararEdicionFacturaAsync(int idFactura);
        Task UpdateAsync(int id, ActualizarFacturaViewModel model);
    }
}