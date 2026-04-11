using Microsoft.AspNetCore.Mvc;
using src.Core.Services.Interfaces;
using src.Presentation.ViewModels.CarritoVM;
using src.Repositories.Interfaces;

namespace src.Presentation.Controllers
{
    public class CarritoController : BaseController
    {
        private readonly ICartService _cartService;
        private readonly IProductoProveedorRepository _prodProvRepo;

        public CarritoController(ICartService cartService, IProductoProveedorRepository prodProvRepo)
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
                SetErrorMessage(result.Message);
                return View(new List<CarritoItemViewModel>());
            }

            // Mapeo DTO → ViewModel
            var viewModels = result.Data.Select(dto => new CarritoItemViewModel
            {
                IdProducto = dto.IdProducto,
                NombreProducto = dto.NombreProducto,
                CodigosExternos = dto.CodigosExternos,
                IdProveedor = dto.IdProveedor,
                NombreProveedor = dto.NombreProveedor,
                PrecioUnitario = dto.PrecioUnitario,
                Cantidad = dto.Cantidad
            }).ToList();

            return View(viewModels);
        }

        [HttpPost]
        public async Task<IActionResult> Agregar(short idProducto, short idProveedor, int cantidad)
        {
            var result = await _cartService.ValidarYAgregarItemAsync(idProducto, idProveedor, cantidad);

            if (!result.Success)
                return BadRequest(new { mensaje = result.Message, errores = result.Errors });

            return Ok(new
            {
                mensaje    = result.Message,
                totalItems = await _cartService.GetCantidadTotalItemsAsync()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Actualizar(short idProducto, short idProveedor, int cantidad)
        {
            var result = await _cartService.ValidarYActualizarCantidadAsync(idProducto, idProveedor, cantidad);

            if (!result.Success)
            {
                SetErrorMessage(result.Message);
                return RedirectToAction(nameof(Index));
            }

            SetSuccessMessage(result.Message);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(short idProducto, short idProveedor)
        {
            var result = await _cartService.RemoverItemAsync(idProducto, idProveedor);
            
            if (!result.Success)
                SetErrorMessage(result.Message);
            else
                SetSuccessMessage(result.Message);

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Acción intermedia que valida el stock real antes de ceder el control
        /// a Compra/Previsualizar. Si hay algún ítem con stock insuficiente,
        /// vuelve al carrito mostrando el detalle del problema.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IrAPrevisualizar(short idProveedor)
        {
            var result = await _cartService.ValidarStockParaPrevisualizarAsync(idProveedor);

            if (!result.Success)
            {
                // Guardamos el mensaje en TempData para que la vista del carrito lo muestre
                TempData["ServiceErrorMessage"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            // Todo ok: redirigimos al flujo normal de compra
            return RedirectToAction("Previsualizar", "Compra", new { idProveedor });
        }
    }
}
