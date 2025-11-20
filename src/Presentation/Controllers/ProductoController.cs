using Microsoft.AspNetCore.Mvc;
using src.Core.Services.Interfaces;
using src.Presentation.ViewModels.ProductoVM;
using src.Repositories.Interfaces;
using System;
using System.Threading.Tasks;

namespace src.Presentation.Controllers
{
    public class ProductoController : Controller
    {
        private readonly IProductoService _productoService;
        private readonly IProductoRepository _repoproducto;

        public ProductoController(IProductoService productoService,IProductoRepository repoprod)
        {
            _productoService = productoService;
            _repoproducto = repoprod;
        }

        
        [HttpGet]
        public async Task<IActionResult> ListarProductos()
        {
            // Obtenemos la lista plana optimizada para la vista
            var productos = await _productoService.GetAllParaListadoAsync();
            return View("ListarProductos", productos);
        }

        [HttpGet]
        public async Task<IActionResult> CrearProducto()
        {
            var vm = await _productoService.PrepararCrearViewModelAsync();
            return View("CrearProducto", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearProducto(CrearProductoViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                await _productoService.RepoblarViewModelAsync(vm);
                return View("CrearProducto", vm);
            }

            try
            {
                await _productoService.CreateAsync(vm);
                TempData["Success"] = "Producto creado exitosamente.";
                return RedirectToAction("ListarProductos");
            }
            catch (ArgumentException ex)
            {
                // Capturamos errores de negocio (ej: "Debe tener al menos un código")
                ModelState.AddModelError(string.Empty, ex.Message);
                await _productoService.RepoblarViewModelAsync(vm);
                return View("CrearProducto", vm);
            }
            catch (Exception ex)
            {
                // Error inesperado
                ModelState.AddModelError(string.Empty, "Ocurrió un error al crear el producto: " + ex.Message);
                await _productoService.RepoblarViewModelAsync(vm);
                return View("CrearProducto", vm);
            }
        }


        [HttpGet]
        public async Task<IActionResult> ActualizarProducto(int id)
        {
            try
            {
                var vm = await _productoService.PrepararActualizarViewModelAsync(id);
                return View("ActualizarProducto", vm);
            }
            catch (KeyNotFoundException)
            {
                TempData["Error"] = "El producto solicitado no existe.";
                return RedirectToAction("ListarProductos");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActualizarProducto(ActualizarProductoViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                 var producto = await _repoproducto.GetByIdAsync(vm.IdProducto);
                 vm.CodigosRegistradosEnBd = producto.CodigoBarra
                .Select(cb => cb.Codigo)
                .ToList();
                await _productoService.RepoblarViewModelAsync(vm);
                return View("ActualizarProducto", vm);
            }

            try
            {
                await _productoService.UpdateAsync(vm);
                TempData["Success"] = "Producto actualizado exitosamente.";
                return RedirectToAction("ListarProductos");
            }
            catch (ArgumentException ex)
            {
                // Aquí capturamos la regla RF 2.3.1 (No dejar sin códigos de barra)
                ModelState.AddModelError(string.Empty, ex.Message); 
                await _productoService.RepoblarViewModelAsync(vm);
                return View("ActualizarProducto", vm);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error al actualizar: " + ex.Message);
                await _productoService.RepoblarViewModelAsync(vm);
                return View("ActualizarProducto", vm);
            }
        }


        [HttpPost] 
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarProducto(int id)
        {
            try
            {
                await _productoService.DeleteAsync(id);
                TempData["Success"] = "Producto eliminado correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "No se pudo eliminar el producto: " + ex.Message;
            }
            
            return RedirectToAction("ListarProductos");
        }
    }
}