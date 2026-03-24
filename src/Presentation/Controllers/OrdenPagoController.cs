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

        [HttpGet("Index/{idProveedor?}")]
        public async Task<IActionResult> Index(short idProveedor)
        {
            var proveedor = await _proveedorService.GetProveedorByIdAsync(idProveedor);
            if (proveedor == null)
                return NotFound();

            var ordenes = await _ordenPagoService.ObtenerPorProveedorAsync(idProveedor);

            var vm = new ModuloOrdenesPagoVM
            {
                IdProveedor = idProveedor,
                RazonSocialProveedor = proveedor.RazonSocial,
                Ordenes = ordenes.Select(o => o.ToIndexVM()).ToList(),
            };

            return View(vm);
        }

        [HttpGet("ObtenerDetalleModal/{id}")]
        public async Task<IActionResult> ObtenerDetalleModal(int id)
        {
            var orden = await _ordenPagoService.ObtenerPorIdAsync(id);
            if (orden == null)
                return NotFound();

            return PartialView("_DetalleOrdenPagoModal", orden.ToDetalleModalVM());
        }

        [HttpGet("Crear/{idProveedor}")]
        public async Task<IActionResult> Crear(short idProveedor)
        {
            var proveedor = await _proveedorService.GetProveedorByIdAsync(idProveedor);
            if (proveedor == null)
                return NotFound();

            var facturas = await _ordenPagoService.ObtenerFacturasPendientesAsync(idProveedor);
            var vm = facturas.ToFormVM(idProveedor, proveedor.RazonSocial);

            return View(vm);
        }

        [HttpPost("Crear/{idProveedor}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(OrdenPagoFormVM vm)
        {
            if (!vm.FacturasDisponibles.Any(x => x.EstaSeleccionada && x.MontoAPagar > 0))
            {
                ModelState.AddModelError(
                    "",
                    "Debe seleccionar al menos una factura y asignar un monto."
                );
                return View(vm);
            }

            if (!ModelState.IsValid)
                return View(vm);

            var orden = vm.ToDomain();
            var resultado = await _ordenPagoService.CrearOrdenAsync(orden);

            if (!resultado.Success)
            {
                ModelState.AddModelError("", resultado.Message);
                return View(vm);
            }

            TempData["Success"] = resultado.Message;
            return RedirectToAction("Index", new { idProveedor = vm.IdProveedor });
        }

        [HttpPost("Eliminar")]
        public async Task<IActionResult> Eliminar(int id, short idProveedor)
        {
            var resultado = await _ordenPagoService.EliminarOrdenAsync(id);

            TempData[resultado.Success ? "Success" : "Error"] = resultado.Message;
            return RedirectToAction("Index", new { idProveedor });
        }

        [HttpPost("Confirmar")]
        public async Task<IActionResult> Confirmar(int id, short idProveedor)
        {
            var resultado = await _ordenPagoService.ConfirmarOrdenAsync(id);

            TempData[resultado.Success ? "Success" : "Error"] = resultado.Message;
            return RedirectToAction("Index", new { idProveedor });
        }
    }
}
