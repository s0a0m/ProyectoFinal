using Microsoft.AspNetCore.Mvc;
using src.Core.Services.Interfaces;
using src.Presentation.Mappers;
using src.Presentation.ViewModels.Comprobantes;

namespace src.Presentation.Controllers
{
    [Route("Comprobantes")]
    public class ComprobantesController : Controller
    {
        private readonly IComprobanteService _comprobanteService;

        public ComprobantesController(IComprobanteService comprobanteService)
        {
            _comprobanteService = comprobanteService;
        }

        [HttpGet("notas/{facturaId}")]
        public async Task<IActionResult> GetAllByFactura(int facturaId)
        {
            var comprobantes = await _comprobanteService.ObtenerPorFacturaAsync(facturaId);
            return Ok(comprobantes.ToListVM());
        }

        [HttpGet("notas/detalle/{id}")]
        public async Task<IActionResult> GetDetalle(int id)
        {
            var comprobante = await _comprobanteService.ObtenerPorIdAsync(id);
            if (comprobante == null)
                return NotFound(new { error = "Comprobante no encontrado" });

            return Ok(comprobante.ToDetalleVM());
        }

        [HttpPost("notas")]
        public async Task<IActionResult> CrearComprobante(
            [FromBody] CrearComprobanteViewModel viewModel
        )
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var comprobante = viewModel.ToDomain();
            if (comprobante is null)
                return BadRequest(new { error = "Tipo de comprobante inválido" });

            var resultado = await _comprobanteService.CrearComprobanteAsync(comprobante);
            if (!resultado.Success)
                return BadRequest(new { error = resultado.Message });

            return Ok(new { message = resultado.Message, idComprobante = resultado.Data });
        }

        [HttpGet("gestion/{idProveedor}")]
        public async Task<IActionResult> GestionNotas(int idProveedor)
        {
            var data = await _comprobanteService.ObtenerGestionNotasAsync(idProveedor);
            if (data == null)
                return NotFound();

            var vm = data.ToGestionVM();
            return View(vm);
        }
    }
}
