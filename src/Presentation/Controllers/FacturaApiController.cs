using Microsoft.AspNetCore.Mvc;
using src.Core.Services.Interfaces;

namespace src.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FacturaApiController : ControllerBase
    {
        private readonly IFacturaService _facturaService;

        public FacturaApiController(IFacturaService facturaService)
        {
            _facturaService = facturaService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var facturaVM = await _facturaService.ObtenerPorIdAsync(id);
                if (facturaVM == null)
                {
                    return NotFound(new { mensaje = $"No se encontró la factura con el ID {id}" });
                }
                return Ok(facturaVM);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    mensaje = $"Error interno al obtener la factura {id}",
                    error = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var facturasVM = await _facturaService.ObtenerTodasAsync();
                return Ok(facturasVM);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno al obtener facturas", error = ex.Message });
            }
        }
    }
}