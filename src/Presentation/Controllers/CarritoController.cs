using Microsoft.AspNetCore.Mvc;
using src.Core.Contracts;
using src.Core.Services.Interfaces;
using src.Presentation.Mappers;
using src.Presentation.ViewModels.CarritoVM;
using src.Repositories.Interfaces;

namespace src.Presentation.Controllers
{
    public class CarritoController : Controller
    {
        private readonly ICartService _cartService;

        public CarritoController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var result = await _cartService.ObtenerCarritoCompletoAsync();

            if (!result.Success)
            {
                // Mensaje específico para la vista del carrito
                TempData["CartError"] = result.Message;
                return View(new List<CarritoItemViewModel>());
            }

            var viewModels = result.Data.Select(dto => dto.ToViewModel()).ToList();
            return View(viewModels);
        }

        [HttpPost]
        public async Task<IActionResult> Agregar(short idProducto, short idProveedor, int cantidad)
        {
            var result = await _cartService.ValidarYAgregarItemAsync(
                idProducto,
                idProveedor,
                cantidad
            );

            if (!result.Success)
            {
                // Devolvemos JSON porque es una llamada AJAX desde el modal
                return BadRequest(new { mensaje = result.Message });
            }

            return Ok(
                new
                {
                    mensaje = result.Message,
                    totalItems = await _cartService.GetCantidadTotalItemsAsync(),
                }
            );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Actualizar(
            short idProducto,
            short idProveedor,
            int cantidad
        )
        {
            var result = await _cartService.ValidarYActualizarCantidadAsync(
                idProducto,
                idProveedor,
                cantidad
            );

            if (!result.Success)
                TempData["CartError"] = result.Message;
            else
                TempData["CartSuccess"] = result.Message;

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(short idProducto, short idProveedor)
        {
            var result = await _cartService.RemoverItemAsync(idProducto, idProveedor);

            if (!result.Success)
                TempData["CartError"] = result.Message;
            else
                TempData["CartSuccess"] = result.Message;

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IrAPrevisualizar(short idProveedor)
        {
            var result = await _cartService.ValidarStockParaPrevisualizarAsync(idProveedor);

            if (!result.Success)
            {
                TempData["CartError"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction("Previsualizar", "Compra", new { idProveedor });
        }
    }
}
