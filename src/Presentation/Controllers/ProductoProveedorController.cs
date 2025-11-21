using Microsoft.AspNetCore.Mvc;
using src.Core.Services.Interfaces;
using src.Presentation.ViewModels.ProductoVM;

namespace src.Presentation.Controllers
{
    public class ProductoProveedorController : Controller
    {
        private readonly IProductoProveedorService _service;

        public ProductoProveedorController(IProductoProveedorService service)
        {
            _service = service;
        }

        // GET: ProductoProveedor
        public async Task<IActionResult> Index()
        {
            var lista = await _service.GetAllParaListadoAsync();
            return View(lista);
        }

        // GET: ProductoProveedor/Create
        public async Task<IActionResult> Create()
        {
            var vm = await _service.PrepararCrearViewModelAsync();
            return View(vm);
        }

        // POST: ProductoProveedor/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CrearProductoProveedorViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                await _service.RepoblarViewModelAsync(vm);
                return View(vm);
            }

            try
            {
                await _service.CreateAsync(vm);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                // Error de negocio (ej. relación ya existente)
                ModelState.AddModelError("", ex.Message);
                await _service.RepoblarViewModelAsync(vm);
                return View(vm);
            }
            catch (Exception ex)
            {
                var mensajeError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        
                ModelState.AddModelError("", $"ERROR TÉCNICO: {mensajeError}");
                // --------------------------------------

                await _service.RepoblarViewModelAsync(vm);
                return View(vm);
            }
        }

        // GET: ProductoProveedor/Edit?idProducto=1&idProveedor=5
        [HttpGet]
        public async Task<IActionResult> Edit(int idProducto, int idProveedor)
        {
            try
            {
                var vm = await _service.PrepararActualizarViewModelAsync(idProducto, idProveedor);
                return View(vm);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        // POST: ProductoProveedor/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ActualizarProductoProveedorViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                // En Edit normalmente no hay listas que repoblar salvo que permitieras cambiar IDs
                return View(vm);
            }

            try
            {
                await _service.UpdateAsync(vm);
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Error al actualizar.");
                return View(vm);
            }
        }

        // POST: ProductoProveedor/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int idProducto, int idProveedor)
        {
            try
            {
                await _service.DeleteAsync(idProducto, idProveedor);
                TempData["Success"] = "Relación eliminada correctamente.";
            }
            catch (Exception)
            {
                TempData["Error"] = "No se pudo eliminar el registro.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}