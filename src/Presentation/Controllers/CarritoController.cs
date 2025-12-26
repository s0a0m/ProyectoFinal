using Microsoft.AspNetCore.Mvc;
using src.Core.Services.Interfaces;
using src.Presentation.ViewModels.CarritoVM;
using src.Repositories.Interfaces;

namespace src.Presentation.Controllers
{
    public class CarritoController : BaseController
    {
        private readonly ICartService _cartService;
        private readonly IProductoProveedorRepository _prodProvRepo; // Para validar datos reales al agregar

        public CarritoController(ICartService cartService, IProductoProveedorRepository prodProvRepo)
        {
            _cartService = cartService;
            _prodProvRepo = prodProvRepo;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Obtenemos todo el carrito plano
            var items = await _cartService.ObtenerCarritoCompletoAsync();

            // La vista se encargará de agrupar visualmente por proveedor
            return View(items);
        }

        [HttpPost]
        public async Task<IActionResult> Agregar(short idProducto, short idProveedor, int cantidad)
        {
            var result = await _cartService.ValidarYAgregarItemAsync(idProducto, idProveedor, cantidad);

            if (!result.Success)
            {
                return BadRequest(new { mensaje = result.Message, errores = result.Errors });
            }

            return Ok(new
            {
                mensaje = result.Message,
                totalItems = await _cartService.GetCantidadTotalItemsAsync()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Actualizar(short idProducto, short idProveedor, int cantidad)
        {
            // if (cantidad == 0)
            // {
            //     await _cartService.RemoverItemAsync(idProducto, idProveedor);
            //     SetSuccessMessage("Producto eliminado del carrito.");
            //     return RedirectToAction(nameof(Index));
            // }

            var result = await _cartService.ValidarYActualizarCantidadAsync(idProducto, idProveedor, cantidad);

            if (!result.Success)
            {
                MapServiceErrors(result);
                return RedirectToAction(nameof(Index));
            }

            SetSuccessMessage(result.Message);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(short idProducto, short idProveedor)
        {
            await _cartService.RemoverItemAsync(idProducto, idProveedor);
            return RedirectToAction(nameof(Index));
        }
    }
}