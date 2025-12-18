using Microsoft.AspNetCore.Mvc;
using src.Core.Services.Interfaces;
using src.Presentation.ViewModels.CarritoVM;
using src.Repositories.Interfaces;

namespace src.Presentation.Controllers
{
    public class CarritoController : Controller
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
            if (cantidad <= 0) return BadRequest("Cantidad inválida");

            // 1. Validar que el producto y precio existan realmente en BD (Seguridad)
            var dto = await _prodProvRepo.GetByIdAsync(idProducto, idProveedor);
            
            if (dto == null) return NotFound("Producto no disponible con este proveedor");
            var relacion = dto.ProductoProveedor;

            // 2. Crear Item para sesión
            var item = new CarritoItemViewModel
            {
                IdProducto = idProducto,
                NombreProducto = relacion.Producto?.Nombre ?? "Producto Desconocido",
                
                // CAMBIO: Asignamos la lista directa (o vacía si es nula)
                CodigosExternos = dto.CodigosBarrasExternos ?? new List<string>(),

                IdProveedor = idProveedor,
                NombreProveedor = relacion.Proveedor?.RazonSocial ?? "Proveedor Desconocido",
                PrecioUnitario = relacion.Precio,
                Cantidad = cantidad
            };

            // 3. Guardar
            await _cartService.AgregarItemAsync(item);

            // Retornamos JSON para que el frontend actualice el icono sin recargar página (AJAX)
            // O redirigimos si prefieres flujo clásico.
            return Ok(new { mensaje = "Agregado", totalItems = await _cartService.GetCantidadTotalItemsAsync() });
        }

        [HttpPost]
        public async Task<IActionResult> Eliminar(short idProducto, short idProveedor)
        {
            await _cartService.RemoverItemAsync(idProducto, idProveedor);
            return RedirectToAction(nameof(Index));
        }
    }
}