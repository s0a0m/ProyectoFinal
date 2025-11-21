using Microsoft.AspNetCore.Mvc;
using src.Core.Services.Interfaces;
using src.Presentation.ViewModels.NovedadesVM;
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

    [HttpPost("crear")]
    public async Task<IActionResult> CrearNovedades([FromBody] NovedadesCrearViewModel novedadVM)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await _novedadesService.CrearNovedadAsync(novedadVM);

            return Ok(new
            {
                message = "Novedad creada con éxito"
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                campo = ex.ParamName,
                error = ex.Message
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                error = "Ocurrió un error inesperado.",
                detalle = ex.Message
            });
        }
    }

}