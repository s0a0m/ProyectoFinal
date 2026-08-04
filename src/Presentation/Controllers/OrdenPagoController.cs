using Microsoft.AspNetCore.Mvc;
using src.Core.Services.Interfaces;
using src.Presentation.Mappers;
using src.Presentation.ViewModels.CuentaCorrienteVM;

namespace src.Presentation.Controllers
{
    [Route("OrdenesPago")]
    public class OrdenPagoController : Controller
    {
        private readonly IOrdenPagoService _ordenPagoService;
        private readonly IProveedorService _proveedorService;

        public OrdenPagoController(
            IOrdenPagoService ordenPagoService,
            IProveedorService proveedorService
        )
        {
            _ordenPagoService = ordenPagoService;
            _proveedorService = proveedorService;
        }

        [HttpGet("Index/{idProveedor}")]
        public async Task<IActionResult> Index(short idProveedor)
        {
            var result = await _ordenPagoService.ObtenerHistorialPagosAsync(idProveedor);

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction("CuentaCorriente", "Proveedor", new { id = idProveedor });
            }

            return View(result.Data.ToViewModel());
        }

        [HttpGet("ObtenerDetalleModal/{id}")]
        public async Task<IActionResult> ObtenerDetalleModal(int id)
        {
            var result = await _ordenPagoService.ObtenerDetalleOrdenAsync(id);

            if (!result.Success)
            {
                return Content(
                    $@"<div class='modal-body text-center py-4'>
                            <i class='bi bi-exclamation-circle text-danger display-4'></i>
                            <p class='mt-2 mb-0'>{result.Message}</p>
                          </div>"
                );
            }

            return PartialView("_DetalleOrdenPagoModal", result.Data.ToDetalleModalVM());
        }

        [HttpGet("Crear/{idProveedor}")]
        public async Task<IActionResult> Crear(short idProveedor)
        {
            var result = await _ordenPagoService.ObtenerDatosParaNuevoPagoAsync(idProveedor);

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction("CuentaCorriente", "Proveedor", new { id = idProveedor });
            }

            return View(result.Data.ToFormVM());
        }

        [HttpPost("Crear/{idProveedor}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(OrdenPagoFormVM vm)
        {
            if (!vm.FacturasDisponibles.Any(x => x.EstaSeleccionada))
            {
                ModelState.AddModelError("", "Debe seleccionar al menos una factura.");
                return View(vm);
            }

            if (!ModelState.IsValid)
                return View(vm);

            var resultado = await _ordenPagoService.CrearOrdenAsync(vm.ToDomain());

            if (!resultado.Success)
            {
                ModelState.AddModelError("", resultado.Message);
                return View(vm);
            }

            TempData["Success"] = resultado.Message;
            return RedirectToAction("Index", new { idProveedor = vm.IdProveedor });
        }

        [HttpPost("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id, short idProveedor)
        {
            var resultado = await _ordenPagoService.EliminarOrdenAsync(id);

            if (resultado.Success)
            {
                TempData["Success"] = resultado.Message;
            }
            else
            {
                TempData["Error"] = resultado.Message;
            }

            return RedirectToAction("Index", new { idProveedor });
        }

        [HttpPost("Confirmar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirmar(int id, short idProveedor)
        {
            var resultado = await _ordenPagoService.ConfirmarOrdenAsync(id);

            if (resultado.Success)
            {
                TempData["Success"] = resultado.Message;
                return RedirectToAction("Index", new { idProveedor });
            }

            TempData["Error"] = resultado.Message;
            return RedirectToAction("Index", new { idProveedor });
        }
    }
}
