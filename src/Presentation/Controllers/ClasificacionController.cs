using Microsoft.AspNetCore.Mvc;
using src.Core.Services.Interfaces;
using src.Presentation.ViewModels.ClasificacionVM;
using src.Presentation.ViewModels.FamiliaVM;
using src.Presentation.ViewModels.CategoriaVM;
using System;
using System.Threading.Tasks;

namespace src.Presentation.Controllers
{
    public class ClasificacionController : Controller
    {
        private readonly IFamiliaService _familiaService;
        private readonly ICategoriaService _categoriaService;

        // Inyectamos AMBOS servicios para orquestar la vista unificada
        public ClasificacionController(IFamiliaService familiaService, ICategoriaService categoriaService)
        {
            _familiaService = familiaService;
            _categoriaService = categoriaService;
        }

        // ==========================================
        // VISTA PRINCIPAL (Dashboard de Clasificación)
        // ==========================================

        // GET: /Clasificacion?familiaId=5
        [HttpGet]
        public async Task<IActionResult> IndexClasificacion(short? familiaId)
        {
            var vm = new GestionarClasificacionViewModel();

            // 1. Cargar todas las familias (para el panel izquierdo/árbol)
            // Usamos GetAllWithCategoriasAsync si quieres mostrar un conteo de hijos, 
            // o simplemente GetAllAsync si solo necesitas los nombres.
            vm.Familias = await _familiaService.GetAllWithCategoriasAsync();

            // 2. Si el usuario seleccionó una familia, cargar sus detalles y categorías hijas
            if (familiaId.HasValue)
            {
                vm.IdFamiliaSeleccionada = familiaId.Value;
                vm.CategoriasSeleccionadas = await _categoriaService.GetByFamiliaIdAsync(familiaId.Value);
            }

            return View(vm);
        }

        // ==========================================
        // ACCIONES DE FAMILIA (CRUD)
        // ==========================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearFamilia(CrearFamiliaViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Datos de familia inválidos.";
                return RedirectToAction("IndexClasificacion"); // En una SPA real usaríamos AJAX, aquí recargamos
            }

            await _familiaService.CreateAsync(vm);
            TempData["Success"] = "Familia creada correctamente.";
            return RedirectToAction("IndexClasificacion");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActualizarFamilia(ActualizarFamiliaViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Datos de actualización inválidos.";
                return RedirectToAction("IndexClasificacion", new { familiaId = vm.IdFamilia });
            }

            try
            {
                await _familiaService.UpdateAsync(vm);
                TempData["Success"] = "Familia actualizada.";
            }
            catch (KeyNotFoundException)
            {
                TempData["Error"] = "La familia no existe.";
            }

            // Redirigimos manteniendo la selección para que el usuario vea los cambios
            return RedirectToAction("IndexClasificacion", new { familiaId = vm.IdFamilia });
        }

        [HttpPost] // Usamos POST para acciones destructivas
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarFamilia(short id)
        {
            try
            {
                await _familiaService.DeleteAsync(id);
                TempData["Success"] = "Familia eliminada.";
                return RedirectToAction("IndexClasificacion"); // Volvemos al inicio (sin selección)
            }
            catch (InvalidOperationException ex)
            {
                // Capturamos la regla de negocio: "No borrar si tiene hijos"
                TempData["Error"] = ex.Message;
                return RedirectToAction("IndexClasificacion", new { familiaId = id }); // Mantenemos selección para que vea el error
            }
        }

        // ==========================================
        // ACCIONES DE CATEGORÍA (CRUD)
        // ==========================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearCategoria(CrearCategoriaViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Datos de categoría inválidos.";
                return RedirectToAction("IndexClasificacion", new { familiaId = vm.IdFamilia });
            }

            try 
            {
                await _categoriaService.CreateAsync(vm);
                TempData["Success"] = "Categoría agregada.";
            }
            catch (ArgumentException ex)
            {
                 TempData["Error"] = ex.Message;
            }

            // Mantenemos al usuario en la familia que estaba editando
            return RedirectToAction("IndexClasificacion", new { familiaId = vm.IdFamilia });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActualizarCategoria(ActualizarCategoriaViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                 TempData["Error"] = "Datos de categoría inválidos.";
                 // Si falla, intentamos volver a la familia original
                 return RedirectToAction("IndexClasificacion", new { familiaId = vm.IdFamilia });
            }

            try
            {
                await _categoriaService.UpdateAsync(vm);
                TempData["Success"] = "Categoría actualizada.";
            }
            catch (Exception ex) // KeyNotFound o ArgumentException
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("IndexClasificacion", new { familiaId = vm.IdFamilia });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarCategoria(short id, short idFamiliaPadre)
        {
            // Nota: Recibimos 'idFamiliaPadre' solo para poder redirigir correctamente a la vista
            try
            {
                await _categoriaService.DeleteAsync(id);
                TempData["Success"] = "Categoría eliminada.";
            }
            catch (InvalidOperationException ex)
            {
                // Regla de negocio: "No borrar si tiene productos"
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("IndexClasificacion", new { familiaId = idFamiliaPadre });
        }
    }
}