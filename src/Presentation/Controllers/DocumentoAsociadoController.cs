using Microsoft.AspNetCore.Mvc;
using src.Core.Services.Implementations;
using src.Core.Services.Interfaces;
using src.Presentation.ViewModels.CuentaCorrienteVM;
namespace src.Presentation.Controllers
{

    [Route("DocumentoAsociado")]
   public class DocumentoAsociadoController: Controller
   {
        
        IDocumentoAsociadoService _documentoAsociadoService;

        public DocumentoAsociadoController(IDocumentoAsociadoService documentoAsociadoService)
        {
            _documentoAsociadoService = documentoAsociadoService;
        }

        [HttpGet("ObtenerDetalleModal/{id}")]
        public async Task<IActionResult> ObtenerDetalleModal(int id)
        {
            var vm = await _documentoAsociadoService.ObtenerDetalleParaModalAsync(id);
            
            if (vm == null) return NotFound();

            return PartialView("_DetalleOrdenPagoModal", vm);
        }



       [HttpGet("Index/{idProveedor?}")]
        public async Task<IActionResult> Index(short idProveedor)
        {
            var vm = await _documentoAsociadoService.ObtenerModuloPorProveedorAsync(idProveedor);
            return View(vm);
        }


       [HttpGet("Crear/{idProveedor}")]
        public async Task<IActionResult> Crear(short idProveedor)
        {
            var vm = await _documentoAsociadoService.ObtenerFormularioCreacionAsync(idProveedor);
            return View(vm);
        }

        [HttpPost("Crear/{idProveedor}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(OrdenPagoFormVM vm)
        {
            // Validacion basica: debe haber al menos una factura seleccionada
            if (!vm.FacturasDisponibles.Any(x => x.EstaSeleccionada && x.MontoAPagar > 0))
            {
                ModelState.AddModelError("", "Debe seleccionar al menos una factura y asignar un monto.");
            }

            if (ModelState.IsValid)
            {
                await _documentoAsociadoService.CrearOrdenAsync(vm);
                return RedirectToAction("Index", new { idProveedor = vm.IdProveedor });
            }

            // Si falla, recargamos la vista (ojo: perderiamos datos complejos si no se maneja bien, 
            // pero por ahora devolvemos el VM tal cual viene del post)
            return View(vm);
        }


        [HttpPost("Eliminar")]
        public async Task<IActionResult> Eliminar(int id,short idProveedor)
        {
            try
            {
                

                await _documentoAsociadoService.EliminarOrdenAsync(id);
                
                TempData["Success"] = "Orden eliminada correctamente.";
                return RedirectToAction("Index",idProveedor);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "No se pudo eliminar la orden: " + ex.Message;
                return RedirectToAction("Index",idProveedor);
            }
        }


        [HttpPost("Confirmar")]
        public async Task<IActionResult> Confirmar(int id, short idProveedor)
        {
            try
            {
                // Asumo que tienes un servicio que pone Enviada = true y descuenta saldos
                await _documentoAsociadoService.ConfirmarOrdenAsync(id); 
                TempData["Success"] = "Orden confirmada y saldos actualizados.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al confirmar: " + ex.Message;
            }
            return RedirectToAction("Index", new { idProveedor });
        }



   } 
}