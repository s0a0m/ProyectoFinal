using Microsoft.AspNetCore.Mvc;
using src.Core.Services.Interfaces; // <-- Este 'using' AHORA SÍ es necesario
using src.Presentation.ViewModels.GrupoPermisoVM;
namespace src.Presentation.Controllers;
public class GrupoPermisosController : Controller
{
    private readonly IGrupoPermisosService _grupoService;

    public GrupoPermisosController(IGrupoPermisosService grupoService)
    {
        _grupoService = grupoService;
    }

    // GET: /GrupoPermisos
    [HttpGet]
    public async Task<IActionResult> ListarGrupos()
    {
        var grupos = await _grupoService.GetAllAsync();
        return View("ListarGrupos", grupos);
    }

    [HttpGet]
    public async Task<IActionResult> VerGrupo(short id)
    {
        try
        {
            // El servicio ahora lanza KeyNotFoundException si no lo encuentra
            var grupo = await _grupoService.GetByIdAsync(id); 
            return View("VerGrupo", grupo); // Pasa el modelo a tu vista
        }
        catch (KeyNotFoundException)
        {
            // Este catch ahora funcionará correctamente
            return NotFound(); 
        }
    }
    // GET: /GrupoPermisos/Crear
    [HttpGet]
    public async Task<IActionResult> CrearGrupo()
    {
        var vm = await _grupoService.PrepararCrearViewModelAsync();
        return View("CrearGrupo", vm);
    }
    
    // POST: /GrupoPermisos/Crear
    [HttpPost]
    public async Task<IActionResult> CrearGrupo(CrearGrupoViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            await _grupoService.RepoblarViewModelParaErrorAsync(vm);
            return View("CrearGrupo", vm);
        }

        await _grupoService.CreateGrupoAsync(vm);
        TempData["realizado"] = "Grupo creado con éxito.";
        return RedirectToAction("ListarGrupos");
    }

    // GET: /GrupoPermisos/Actualizar/5
    [HttpGet]
    public async Task<IActionResult> ActualizarGrupo(short id)
    {
        try
        {
            var vm = await _grupoService.PrepararActualizarViewModelAsync(id);
            return View("ActualizarGrupo", vm);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // POST: /GrupoPermisos/Actualizar
    [HttpPost]
    public async Task<IActionResult> ActualizarGrupo(ActualizarGrupoViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            await _grupoService.RepoblarViewModelParaErrorAsync(vm);
            return View("ActualizarGrupo", vm);
        }

        try
        {
            await _grupoService.UpdateGrupoAsync(vm);
            TempData["realizado"] = "Grupo actualizado con éxito.";
            return RedirectToAction("ListarGrupos");
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
    
    // GET: /GrupoPermisos/Eliminar/5
    [HttpGet] 
    public async Task<IActionResult> EliminarGrupo(short id)
    {
        // (En una app real, esto debería ser un POST)
        await _grupoService.DeleteGrupoAsync(id);
        TempData["realizado"] = "Grupo eliminado con éxito.";
        return RedirectToAction("ListarGrupos");
    }
}