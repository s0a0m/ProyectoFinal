using Microsoft.AspNetCore.Mvc;
using src.Core.Services.Interfaces;
using src.Repositories.Interfaces;
namespace src.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NovedadesControler : ControllerBase
{
    private readonly INovedadesService _novedadesService;

    public NovedadesControler(INovedadesService novedadesService)
    {
        _novedadesService = novedadesService;
    }
    [HttpGet("novedades")]
    public async Task<IActionResult> ListarNovedades()
    {
        var grupos = await _novedadesService.GetAllNovedadesPendientes();
        return Ok(grupos);
    }
}