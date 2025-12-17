using Microsoft.AspNetCore.Mvc;
using src.Core.Services.Interfaces;
using src.Presentation.ViewModels.NovedadesVM;
using src.Repositories.Interfaces;
namespace src.Presentation.Controllers;

using Microsoft.AspNetCore.Session;
using Microsoft.AspNetCore.Http;
using src.Presentation.Attributes;

public class NovedadesController : Controller
{
    private readonly INovedadesService _novedadesService;

    public NovedadesController(INovedadesService novedadesService)
    {
        _novedadesService = novedadesService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var novedades = await _novedadesService.GetAllNovedadesPendientes();
        return View(novedades);
    }

    [HttpPost]
    public async Task<IActionResult> CrearNovedades(NovedadesCrearViewModel novedadVM)
    {
        if (!ModelState.IsValid)
        {
            var lista = await _novedadesService.GetAllNovedadesPendientes();
            return View("Index", lista);
        }
        try
        {
            await _novedadesService.CrearNovedadAsync(novedadVM);
            TempData["Success"] = "Novedad creada con éxito";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }

}