using Microsoft.AspNetCore.Mvc;
using src.Core.Services.Interfaces;
using src.Presentation.ViewModels.FacturaVM;

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
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ActualizarFacturaViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var resultado = await _facturaService.UpdateAsync(id, modelo);

                if (!resultado)
                {
                    return NotFound(new { mensaje = $"No se pudo actualizar: La factura con ID {id} no existe." });
                }

                return Ok(new { mensaje = "Factura actualizada exitosamente" });
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("ya existe"))
                {
                    return BadRequest(new { mensaje = ex.Message });
                }

                return StatusCode(500, new
                {
                    mensaje = "Error interno al intentar actualizar la factura",
                    error = ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CrearFacturaViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var existe = await _facturaService.ExisteNumeroFacturaAsync((short)modelo.IdProveedor, modelo.NumeroFactura);
                if (existe)
                {
                    return BadRequest(new { mensaje = "El número de factura ya se encuentra registrado para este proveedor." });
                }

                var idGenerado = await _facturaService.CrearFacturaAsync(modelo);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = idGenerado },
                    new { id = idGenerado, mensaje = "Factura creada exitosamente" }
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    mensaje = "Ocurrió un error al procesar la factura",
                    error = ex.Message
                });
            }
        }
        [HttpGet("pendientes")]
        public async Task<IActionResult> GetPendientes()
        {
            try
            {
                var facturas = await _facturaService.ObtenerPendientesPagoAsync();
                return Ok(facturas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener facturas pendientes", error = ex.Message });
            }
        }

        [HttpGet("existe/{idProveedor}/{numero}")]
        public async Task<IActionResult> ExisteNumero(short idProveedor, string numero)
        {
            try
            {
                var existe = await _facturaService.ExisteNumeroFacturaAsync(idProveedor, numero);
                return Ok(new { existe });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al validar número de factura", error = ex.Message });
            }
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