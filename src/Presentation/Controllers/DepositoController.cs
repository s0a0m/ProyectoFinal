using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using src.Presentation.ViewModels.DepositoVM;
using src.Core.Services.Interfaces;
using src.Presentation.Attributes;

namespace src.Presentation.Controllers;

[AuthorizePermiso("P13_ABM_Deposito")]
public class DepositoController : Controller
{
    private readonly IDepositoService _depositoService;

    public DepositoController(IDepositoService depositoService)
    {
        _depositoService = depositoService;
    }

    // ── Index / Listado ───────────────────────────────────────────────────────

    [HttpGet]
    [HttpGet]
public async Task<IActionResult> Index()
    {
        try
        {
            var depositos  = await _depositoService.GetAllAsync();
            var provincias = await _depositoService.GetProvinciasAsync();

            var vm = new DepositoIndexViewModel
            {
                Depositos = depositos,
                Formulario = new DepositoFormViewModel
                {
                    Provincias = provincias
                }
            };
            return View(vm);
        }
        catch (Exception)
        {
            TempData["error"] = "Ocurrió un error al cargar los depósitos.";
            return View(new DepositoIndexViewModel());
        }
    } 

    [HttpGet]
    public async Task<IActionResult> VerDetalle(int id)
    {
        try
        {
            var vm = await _depositoService.GetDetalleAsync(id);
            if (vm is null)
            {
                TempData["error"] = "No se encontró el depósito solicitado.";
                return RedirectToAction(nameof(Index));
            }
            return View(vm);
        }
        catch (Exception)
        {
            TempData["error"] = "Error al cargar el detalle del depósito.";
            return RedirectToAction(nameof(Index));
        }
    }


    // ── Crear ─────────────────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> CrearDeposito()
    {
        try
        {
            var vm = await _depositoService.GetFormularioVacioAsync();
            return View(vm);
        }
        catch (Exception)
        {
            TempData["error"] = "Error al cargar el formulario de nuevo depósito.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CrearDeposito(DepositoFormViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            await _depositoService.CargarOpcionesAsync(vm);
            return View(vm);
        }

        try
        {
            var (success, message) = await _depositoService.CreateAsync(vm);

            if (success)
            {
                TempData["realizado"] = message;
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, message);
            await _depositoService.CargarOpcionesAsync(vm);
            return View(vm);
        }
        catch (Exception)
        {
            TempData["error"] = "Error inesperado al crear el depósito.";
            return RedirectToAction(nameof(Index));
        }
    }

    // ── Actualizar ────────────────────────────────────────────────────────────

    // [HttpGet]
    // public async Task<IActionResult> ActualizarDeposito(int id)
    // {
    //     try
    //     {
    //         var vm = await _depositoService.GetFormularioAsync(id);
    //         if (vm is null)
    //         {
    //             TempData["error"] = "No se encontró el depósito solicitado.";
    //             return RedirectToAction(nameof(Index));
    //         }
    //         return View(vm);
    //     }
    //     catch (Exception)
    //     {
    //         TempData["error"] = "Error al cargar el depósito para editar.";
    //         return RedirectToAction(nameof(Index));
    //     }
    // }

   [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ActualizarDeposito(DepositoFormViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            TempData["error"] = "Datos inválidos: " +
                string.Join(", ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));

            return vm.IdDeposito > 0
                ? RedirectToAction(nameof(VerDetalle), new { id = vm.IdDeposito })
                : RedirectToAction(nameof(Index));
        }

        try
        {
            var (success, message) = await _depositoService.UpdateAsync(vm);
            TempData[success ? "realizado" : "error"] = message;

            return vm.IdDeposito > 0
                ? RedirectToAction(nameof(VerDetalle), new { id = vm.IdDeposito })
                : RedirectToAction(nameof(Index));
        }
        catch (Exception)
        {
            TempData["error"] = "Error inesperado al actualizar el depósito.";
            return vm.IdDeposito > 0
                ? RedirectToAction(nameof(VerDetalle), new { id = vm.IdDeposito })
                : RedirectToAction(nameof(Index));
        }
    }

    // ── Eliminar / Reactivar ──────────────────────────────────────────────────

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EliminarDeposito(int id)
    {
        try
        {
            var (success, message) = await _depositoService.DeleteAsync(id);
            TempData[success ? "realizado" : "error"] = message;
        }
        catch (Exception)
        {
            TempData["error"] = "Error inesperado al dar de baja el depósito.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ReactivarDeposito(int id)
    {
        try
        {
            var (success, message) = await _depositoService.ReactivateAsync(id);
            TempData[success ? "realizado" : "error"] = message;
        }
        catch (Exception)
        {
            TempData["error"] = "Error inesperado al reactivar el depósito.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> GetDatosJson(int id)
    {
        try
        {
            var vm = await _depositoService.GetFormularioAsync(id);
            if (vm is null) return NotFound();
            return Json(new
            {
                vm.IdDeposito,
                vm.Nombre,
                vm.Calle,
                vm.Numero,
                vm.Piso,
                vm.Comentario,
                vm.IdProvincia,
                vm.Activo
            });
        }
        catch (Exception)
        {
            return StatusCode(500);
        }
    }

}