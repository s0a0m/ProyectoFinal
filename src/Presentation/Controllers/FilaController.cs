using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using src.Presentation.ViewModels.FilaVM;

using src.Core.Services.Interfaces;
using src.Presentation.Attributes;
namespace src.Controllers;

[AuthorizePermiso("P13_ABM_Deposito")]
public class FilaController : Controller
{
    private readonly IFilaService _filaService;

    public FilaController(IFilaService filaService)
    {
        _filaService = filaService;
    }

    // ── Listado por estante ───────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> VerFilas(int idEstante)
    {
        try
        {
            var filas = await _filaService.GetByEstanteAsync(idEstante);
            ViewBag.IdEstante      = idEstante;
            ViewBag.NumeroEstante  = filas.FirstOrDefault()?.NumeroEstante  ?? "—";
            ViewBag.IdDeposito     = filas.FirstOrDefault()?.IdDeposito     ?? 0;
            ViewBag.NombreDeposito = filas.FirstOrDefault()?.NombreDeposito ?? "—";

            return View(filas);
        }
        catch (Exception)
        {
            TempData["error"] = "Error al cargar las filas del estante.";
            return RedirectToAction("Index", "Deposito");
        }
    }

    // ── Crear ─────────────────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> CrearFila(int? idEstante)
    {
        try
        {
            var vm = await _filaService.GetFormularioVacioAsync(idEstante);
            return View(vm);
        }
        catch (Exception)
        {
            TempData["error"] = "Error al cargar el formulario de nueva fila.";
            return RedirectToAction("Index", "Deposito");
        }
    }

   [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CrearFila(FilaFormViewModel vm)
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
            var (success, message) = await _filaService.CreateAsync(vm);
            TempData[success ? "realizado" : "error"] = message;
            return RedirectToAction("VerDetalle", "Deposito", new { id = vm.IdDeposito });
        }
        catch (Exception)
        {
            TempData["error"] = "Error inesperado al crear la fila.";
            return RedirectToAction("VerDetalle", "Deposito", new { id = vm.IdDeposito });
        }
    }

    // ── Actualizar ────────────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> ActualizarFila(int id)
    {
        try
        {
            var vm = await _filaService.GetFormularioAsync(id);
            if (vm is null)
            {
                TempData["error"] = "No se encontró la fila solicitada.";
                return RedirectToAction("Index", "Deposito");
            }
            return View(vm);
        }
        catch (Exception)
        {
            TempData["error"] = "Error al cargar la fila para editar.";
            return RedirectToAction("Index", "Deposito");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ActualizarFila(FilaFormViewModel vm)
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
            var (success, message) = await _filaService.UpdateAsync(vm);
            TempData[success ? "realizado" : "error"] = message;
            return RedirectToAction("VerDetalle", "Deposito", new { id = vm.IdDeposito });
        }
        catch (Exception)
        {
            TempData["error"] = "Error inesperado al actualizar la fila.";
            return RedirectToAction("VerDetalle", "Deposito", new { id = vm.IdDeposito });
        }
    }

    // ── Eliminar / Reactivar ──────────────────────────────────────────────────

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EliminarFila(int id, int idEstante, int idDeposito)
    {
        try
        {
            var (success, message) = await _filaService.DeleteAsync(id);
            TempData[success ? "realizado" : "error"] = message;
        }
        catch (Exception)
        {
            TempData["error"] = "Error inesperado al dar de baja la fila.";
        }

        return RedirectToAction("VerDetalle", "Deposito", new { id = idDeposito });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ReactivarFila(int id, int idEstante, int idDeposito)
    {
        try
        {
            var (success, message) = await _filaService.ReactivateAsync(id);
            TempData[success ? "realizado" : "error"] = message;
        }
        catch (Exception)
        {
            TempData["error"] = "Error inesperado al reactivar la fila.";
        }

         return RedirectToAction("VerDetalle", "Deposito", new { id = idDeposito });
    }
}