using Microsoft.AspNetCore.Mvc;
using src.Core.Services.Interfaces;
using src.Presentation.ViewModels.FacturaVM;
using src.Repositories.Interfaces;

namespace src.Presentation.Controllers
{
    public class FacturaController : Controller
    {
        private readonly IFacturaService _facturaService;
        private readonly IFacturaRepository _facturaRepo;

        public FacturaController(IFacturaService facturaService,IFacturaRepository facturaRepo)
        {
            _facturaService = facturaService;
            _facturaRepo = facturaRepo;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var todas = await _facturaService.ObtenerTodasAsync();
                return View(todas);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Ocurrió un error inesperado: {ex.Message}";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var vm = await _facturaService.ObtenerPorIdAsync(id);
                if (vm == null) return NotFound();
                return View(vm);
            }
            catch (KeyNotFoundException)
            {
                TempData["Error"] = "No se encontro la factura seleccionada";
                return View("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "No se encontro la factura seleccionada" + ex.Message;
                return View("Index");
            }
        }



        [HttpGet]
        public async Task<IActionResult> CreateFromCompra(int idCompra)
        {
            try
            {
                var vm = await _facturaService.PrepararFacturaDesdeCompraAsync(idCompra);
                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "Compra"); // O donde corresponda
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(CrearFacturaViewModel model)
        {
            if (model.TipoCondicion == "Cuota")
            {
                if (!model.CantidadCuotas.HasValue || model.CantidadCuotas < 1)
                    ModelState.AddModelError("CantidadCuotas", "Debe especificar la cantidad de cuotas.");
                
                if (!model.InteresPorcentual.HasValue)
                    ModelState.AddModelError("InteresPorcentual", "Debe especificar el interés (ponga 0 si es sin interés).");
            }
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(kvp => kvp.Value.Errors.Count > 0)
                    .Select(kvp => new {
                        Key = kvp.Key,
                        Errors = kvp.Value.Errors.Select(e => string.IsNullOrEmpty(e.ErrorMessage) ? (e.Exception?.Message ?? "parse error") : e.ErrorMessage).ToArray()
                    }).ToArray();

                TempData["ModelStateDebug"] = System.Text.Json.JsonSerializer.Serialize(errors);
                return View("CreateFromCompra", model);
            }

            try
            {
                // Validar duplicado de nro factura (opcional si no está en el service)
                bool existe = await _facturaService.ExisteNumeroFacturaAsync((short)model.IdProveedor, model.NumeroFactura);
                if (existe)
                {
                    ModelState.AddModelError("NumeroFactura", "Este número de factura ya existe para este proveedor.");
                    return View("CreateFromCompra", model);
                }

                int idFactura = await _facturaService.CrearFacturaAsync(model);
                TempData["Success"] = $"Factura #{model.NumeroFactura} registrada correctamente.";
                return RedirectToAction("Details", new { id = idFactura });
            }
            catch (Exception ex)
            {
                var errorReal = ex.InnerException?.Message ?? ex.Message;

                ModelState.AddModelError("", "Error al crear la factura: " + errorReal);

                return View("CreateFromCompra", model);
            }

        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var vm = await _facturaService.PrepararEdicionFacturaAsync(id);
                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar la factura: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ActualizarFacturaViewModel model)
        {
            // 1. Validaciones manuales de Cuotas (igual que en crear)
            if (model.TipoCondicion == "Cuota")
            {
                if (!model.CantidadCuotas.HasValue || model.CantidadCuotas < 1)
                    ModelState.AddModelError("CantidadCuotas", "Debe especificar la cantidad de cuotas.");
                if (!model.InteresPorcentual.HasValue)
                    ModelState.AddModelError("InteresPorcentual", "Debe especificar el interés.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // 2. Validar duplicado excluyendo la actual
                // Nota: Asegúrate de haber actualizado la interfaz IFacturaService con este método
                bool existe = await _facturaRepo.ExisteNumeroFacturaAsync((short)model.IdProveedor, model.NumeroFactura, model.IdFactura); // Pasamos el ID a excluir
                
                // *Si no actualizaste el service para aceptar el ID excluir, deberás hacerlo en el Service Implementation*
                // var existe = await _facturaRepository.ExisteNumeroFacturaAsync((short)model.IdProveedor, model.NumeroFactura, model.IdFactura); 
                
                if (existe)
                {
                    ModelState.AddModelError("NumeroFactura", "Este número de factura ya existe.");
                    return View(model);
                }

                await _facturaService.UpdateAsync(model.IdFactura, model);
                TempData["Success"] = "Factura actualizada correctamente.";
                return RedirectToAction("Details", new { id = model.IdFactura });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al actualizar: " + (ex.InnerException?.Message ?? ex.Message));
                return View(model);
            }
        }




    }






}