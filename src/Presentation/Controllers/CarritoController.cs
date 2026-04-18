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
        private readonly IProductoProveedorRepository _prodProvRepo;

        public CarritoController(
            ICartService cartService,
            IProductoProveedorRepository prodProvRepo
        )
        {
            _cartService = cartService;
            _prodProvRepo = prodProvRepo;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var result = await _cartService.ObtenerCarritoCompletoAsync();

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
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
                return BadRequest(new { mensaje = result.Message, errores = result.Errors });

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
            {
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(short idProducto, short idProveedor)
        {
            var result = await _cartService.RemoverItemAsync(idProducto, idProveedor);

            if (!result.Success)
                TempData["Error"] = result.Message;
            else
                TempData["Success"] = result.Message;

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IrAPrevisualizar(short idProveedor)
        {
            var result = await _cartService.ValidarStockParaPrevisualizarAsync(idProveedor);

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction("Previsualizar", "Compra", new { idProveedor });
        }
    }
}
