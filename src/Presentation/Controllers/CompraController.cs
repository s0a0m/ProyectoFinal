using Microsoft.AspNetCore.Mvc;
using src.Core.Services.Interfaces;
using src.Models.Common;
using src.Models.Domain;
using src.Presentation.Attributes;
using src.Presentation.Mappers;
using src.Presentation.ViewModels.CarritoVM;
using src.Presentation.ViewModels.CompraVM;
using src.Repositories.Interfaces;

namespace src.Presentation.Controllers
{
    public class CompraController : Controller
    {
        private readonly ICompraService _compraService;
        private readonly ICartService _cartService;
        private readonly IUserService _userService;

        public CompraController(
            ICompraService compraService,
            ICartService cartService,
            IUserService userService
        )
        {
            _compraService = compraService;
            _cartService = cartService;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var compras = await _compraService.GetAllAsync();
            return View(
                compras.OrderByDescending(c => c.FechaCompra).Select(c => c.ToListarVM()).ToList()
            );
        }

        [HttpGet]
        public async Task<IActionResult> Previsualizar(short idProveedor)
        {
            var result = await _cartService.ObtenerItemsPorProveedorAsync(idProveedor);
            if (!result.Success || !result.Data.Any())
                return RedirectToAction("Index", "Carrito");

            return View(CompraMapper.ToPrevisualizarVM(idProveedor, result.Data));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirmar(ConfirmarCompraViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var cart = await _cartService.ObtenerItemsPorProveedorAsync(model.IdProveedor);
                model.Items = cart.Data.Select(d => d.ToViewModel()).ToList();
                return View("Previsualizar", model);
            }

            var usuario = _userService.ObtenerUsuarioActual();
            if (usuario == null)
                return RedirectToAction("Login", "Acceso");

            // Llamada limpia al servicio:
            var result = await _compraService.ProcesarCompraDesdeCarritoAsync(
                model.IdProveedor,
                usuario.IdUsuario,
                model.Observaciones
            );

            if (result.Success)
            {
                TempData["Success"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = result.Message;
            return RedirectToAction("Previsualizar", new { idProveedor = model.IdProveedor });
        }

        [HttpGet]
        public async Task<IActionResult> Gestionar(int id)
        {
            var result = await _compraService.GetByIdAsync(id);
            if (!result.Success)
                return NotFound();
            return View(result.Data.ToGestionarVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GuardarCambios(GestionarCompraViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var current = await _compraService.GetByIdAsync(model.IdCompra);
                return View("Gestionar", current.Data.ToGestionarVM());
            }

            var result = await _compraService.UpdateAsync(model.ToDomain());

            if (result.Success)
            {
                TempData["Success"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = result.Message;
            return RedirectToAction(nameof(Gestionar), new { id = model.IdCompra });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarcarEnviada(int id)
        {
            var result = await _compraService.MarcarEnviadaAsync(id);
            TempData[result.Success ? "Success" : "Error"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancelar(int id, string motivo)
        {
            var result = await _compraService.CancelarCompraAsync(id, motivo);
            TempData[result.Success ? "Success" : "Error"] = result.Message;
            return RedirectToAction(nameof(Index));
        }
    }
}
