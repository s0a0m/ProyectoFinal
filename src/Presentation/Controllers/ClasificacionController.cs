using Microsoft.AspNetCore.Mvc;
using src.Core.Services.Interfaces;
using src.Presentation.ViewModels.ClasificacionVM;
using src.Presentation.ViewModels.FamiliaVM;
using src.Presentation.ViewModels.CategoriaVM;
using src.Presentation.Attributes;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Session;
using Microsoft.AspNetCore.Http;

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

     
        [HttpGet]
        [AuthorizePermiso("P12_ABM_CLASIFICACION_PRODUCTOS")]
        public async Task<IActionResult> IndexClasificacion(short? familiaId)
        {
            var vm = new GestionarClasificacionViewModel();

          
            vm.Familias = await _familiaService.GetAllWithCategoriasAsync();

           
            if (familiaId.HasValue)
            {
                vm.IdFamiliaSeleccionada = familiaId.Value;
                vm.CategoriasSeleccionadas = await _categoriaService.GetByFamiliaIdAsync(familiaId.Value);
            }

            return View(vm);
        }

        // ACCIONES DE FAMILIA (CRUD)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermiso("P12_ABM_CLASIFICACION_PRODUCTOS")]
        public async Task<IActionResult> CrearFamilia(CrearFamiliaViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Datos de familia inválidos.";
                return RedirectToAction("IndexClasificacion"); 
            }
            try
            {
                await _familiaService.CreateAsync(vm);
                TempData["Success"] = "Familia creada correctamente.";
                return RedirectToAction("IndexClasificacion");
            }
            catch (ArgumentException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("IndexClasificacion");
            }
            catch (Exception ex)
            {
                // Error inesperado
                TempData["Error"] = "Error inesperado: " + ex.Message;
                return RedirectToAction("IndexClasificacion");
            }
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermiso("P12_ABM_CLASIFICACION_PRODUCTOS")]
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
            }catch(ArgumentException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("IndexClasificacion");
            }catch(Exception ex)
            {
                TempData["Error"] = "Error inesperado: " + ex.Message;
                return RedirectToAction("IndexClasificacion");
            }

            
            return RedirectToAction("IndexClasificacion", new { familiaId = vm.IdFamilia });
        }

        [HttpPost] 
        [ValidateAntiForgeryToken]
        [AuthorizePermiso("P12_ABM_CLASIFICACION_PRODUCTOS")]
        public async Task<IActionResult> EliminarFamilia(short id)
        {
            try
            {
                await _familiaService.DeleteAsync(id);
                TempData["Success"] = "Familia eliminada.";
                return RedirectToAction("IndexClasificacion"); 
            }
            catch (InvalidOperationException ex)
            {
               
                TempData["Error"] = ex.Message;
                return RedirectToAction("IndexClasificacion", new { familiaId = id }); // 
            }
        }

       
        // ACCIONES DE CATEGORÍA (CRUD)
    

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermiso("P12_ABM_CLASIFICACION_PRODUCTOS")]
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
        [AuthorizePermiso("P12_ABM_CLASIFICACION_PRODUCTOS")]
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
        [AuthorizePermiso("P12_ABM_CLASIFICACION_PRODUCTOS")]
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