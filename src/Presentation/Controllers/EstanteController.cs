using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using src.Presentation.ViewModels.EstanteVM;
using src.Core.Services.Interfaces;
using src.Presentation.Attributes;


namespace src.Controllers;

[AuthorizePermiso("P13_ABM_Deposito")]
public class EstanteController : Controller
{
    private readonly IEstanteService _estanteService;

    public EstanteController(IEstanteService estanteService)
    {
        _estanteService = estanteService;
    }

    // ── Listado por depósito ──────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> VerEstantes(int idDeposito)
    {
        try
        {
            var estantes = await _estanteService.GetByDepositoAsync(idDeposito);
            ViewBag.IdDeposito = idDeposito;

            // Nombre del depósito (tomado del primer item si existe)
            ViewBag.NombreDeposito = estantes.FirstOrDefault()?.NombreDeposito ?? "—";

            return View(estantes);
        }
        catch (Exception)
        {
            TempData["error"] = "Error al cargar los estantes del depósito.";
            return RedirectToAction("Index", "Deposito");
        }
    }

    // ── Crear ─────────────────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> CrearEstante(int? idDeposito)
    {
        try
        {
            var vm = await _estanteService.GetFormularioVacioAsync(idDeposito);
            return View(vm);
        }
        catch (Exception)
        {
            TempData["error"] = "Error al cargar el formulario de nuevo estante.";
            return RedirectToAction("VerDetalle", "Deposito", new { id = idDeposito });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CrearEstante(EstanteFormViewModel vm)
    {
        // Validación manual en lugar de return View(vm)
        if (!ModelState.IsValid)
        {
            TempData["error"] = "Datos inválidos: " +
                string.Join(", ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
            return RedirectToAction("VerDetalle", "Deposito", new { id = vm.IdDeposito });
        }

        try
        {
            var (success, message) = await _estanteService.CreateAsync(vm);
            TempData[success ? "realizado" : "error"] = message;
            return RedirectToAction("VerDetalle", "Deposito", new { id = vm.IdDeposito });
        }
        catch (Exception ex)
        {
            TempData["error"] = $"[DEBUG] {ex.GetType().Name}: {ex.Message} | Inner: {ex.InnerException?.Message}";
            return RedirectToAction("VerDetalle", "Deposito", new { id = vm.IdDeposito });
        }
    }

    // ── Actualizar ────────────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> ActualizarEstante(int id)
    {
        try
        {
            var vm = await _estanteService.GetFormularioAsync(id);
            if (vm is null)
            {
                TempData["error"] = "No se encontró el estante solicitado.";
                return RedirectToAction("Index", "Deposito");
            }
            return View(vm);
        }
        catch (Exception)
        {
            TempData["error"] = "Error al cargar el estante para editar.";
            return RedirectToAction("Index", "Deposito");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ActualizarEstante(EstanteFormViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            TempData["error"] = "Datos inválidos: " +
                string.Join(", ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
            return RedirectToAction("VerDetalle", "Deposito", new { id = vm.IdDeposito });
        }

        try
        {
            var (success, message) = await _estanteService.UpdateAsync(vm);
            TempData[success ? "realizado" : "error"] = message;
            return RedirectToAction("VerDetalle", "Deposito", new { id = vm.IdDeposito });
        }
        catch (Exception)
        {
            TempData["error"] = "Error inesperado al actualizar el estante.";
            return RedirectToAction("VerDetalle", "Deposito", new { id = vm.IdDeposito });
        }
    }

    // ── Eliminar / Reactivar ──────────────────────────────────────────────────

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EliminarEstante(int id, int idDeposito)
    {
        try
        {
            var (success, message) = await _estanteService.DeleteAsync(id);
            TempData[success ? "realizado" : "error"] = message;
        }
        catch (Exception)
        {
            TempData["error"] = "Error inesperado al dar de baja el estante.";
        }

        return RedirectToAction("VerDetalle", "Deposito", new { id = idDeposito });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ReactivarEstante(int id, int idDeposito)
    {
        try
        {
            var (success, message) = await _estanteService.ReactivateAsync(id);
            TempData[success ? "realizado" : "error"] = message;
        }
        catch (Exception)
        {
            TempData["error"] = "Error inesperado al reactivar el estante.";
        }

        return RedirectToAction("VerDetalle", "Deposito", new { id = idDeposito });
    }

    // ── Marcar espacio ────────────────────────────────────────────────────────

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarcarEspacio(int id, int idDeposito, bool tieneEspacio)
    {
        try
        {
            var (success, message) = await _estanteService.MarcarEspacioAsync(id, tieneEspacio);
            TempData[success ? "realizado" : "error"] = message;
        }
        catch (Exception)
        {
            TempData["error"] = "Error al actualizar el espacio del estante.";
        }

        return RedirectToAction(nameof(VerEstantes), new { idDeposito });
    }
}