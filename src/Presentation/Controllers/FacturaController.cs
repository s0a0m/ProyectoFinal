using Microsoft.AspNetCore.Mvc;
using src.Core.Services.Interfaces;

namespace src.Presentation.Controllers
{
    public class FacturaController : Controller
    {
        private readonly IFacturaService _facturaService;

        public FacturaController(IFacturaService facturaService)
        {
            _facturaService = facturaService;
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






    }






}