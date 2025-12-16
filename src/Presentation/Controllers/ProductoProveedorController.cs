using Microsoft.AspNetCore.Mvc;
using src.Core.Services.Interfaces;
using src.Presentation.ViewModels.ProductoVM;
using Microsoft.AspNetCore.Session;
using src.Presentation.Attributes;
using Microsoft.AspNetCore.Http;
namespace src.Presentation.Controllers
{
    public class ProductoProveedorController : Controller
    {
        private readonly IProductoProveedorService _service;

        public ProductoProveedorController(IProductoProveedorService service)
        {
            _service = service;
        }

        [HttpGet]
        [AuthorizePermiso("P08_VER_LISTA_COMPRAS")]
        public async Task<IActionResult> Index()
        {
            var lista = await _service.GetAllParaListadoAsync();
            return View(lista);
        }

        [HttpGet]
        [AuthorizePermiso("P07_GESTION_COMPRAS")]
        public async Task<IActionResult> Create()
        {
            var vm = await _service.PrepararCrearViewModelAsync();
            return View(vm);
        }

        // POST: ProductoProveedor/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermiso("P07_GESTION_COMPRAS")]
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
                TempData["Success"] = "Relación creada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                // Error de negocio (ej. relación ya existente)
                ModelState.AddModelError("", ex.Message);
                await _service.RepoblarViewModelAsync(vm);
                return View(vm);
            }
            catch (ArgumentException ex) 
            {
                ModelState.AddModelError("", ex.Message);
                await _service.RepoblarViewModelAsync(vm);
                return View(vm);
            }
            catch (Exception ex)
            {
                var mensajeError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        
                ModelState.AddModelError("", $"ERROR TÉCNICO: {mensajeError}");
                await _service.RepoblarViewModelAsync(vm);
                return View(vm);
            }
        }

       
        [HttpGet]
        [AuthorizePermiso("P07_GESTION_COMPRAS")]
        public async Task<IActionResult> Edit(int idProducto, int idProveedor)
        {
            try
            {
                var vm = await _service.PrepararActualizarViewModelAsync(idProducto, idProveedor);
                return View(vm);
            }
            catch (KeyNotFoundException)
            {
                TempData["Error"] = "La relación solicitada no existe.";
                return RedirectToAction(nameof(Index));
            }
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermiso("P07_GESTION_COMPRAS")]
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
                TempData["Success"] = "Relación actualizada correctamente.";
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

    }
}