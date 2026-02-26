using src.Presentation.ViewModels.CuentaCorrienteVM;

namespace src.Core.Services.Interfaces
{
    public interface IDocumentoAsociadoService
    {
        Task<OrdenPagoDetalleModalVM?> ObtenerDetalleParaModalAsync(int idOrden);
        Task<ModuloOrdenesPagoVM> ObtenerModuloPorProveedorAsync(short idProveedor);
        Task<OrdenPagoFormVM> ObtenerFormularioCreacionAsync(short idProveedor);

        Task CrearOrdenAsync(OrdenPagoFormVM vm);
        Task EliminarOrdenAsync(int idOrden);
         Task ConfirmarOrdenAsync(int idOrden);
    }
}