using Microsoft.AspNetCore.Mvc;
using src.Core.Services.Interfaces;
using src.Models.Domain;
using src.Presentation.ViewModels.CompraVM;

namespace src.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompraController : ControllerBase
    {
        private readonly ICompraService _compraService;

        public CompraController(ICompraService compraService)
        {
            _compraService = compraService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ListarCompraViewModel>>> Index()
        {
            var compras = await _compraService.GetAllAsync();

            return Ok(compras);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<ListarCompraViewModel>> GetById(short id)
        {
            var compra = await _compraService.GetByIdAsync(id);
            if (compra == null)
            {
                return NotFound(new { mensaje = $"La compra con ID {id} no fue encontrada." });
            }
            return Ok(compra);
        }
        [HttpPost]
        public async Task<ActionResult<int>> Create([FromBody] CrearCompraViewModel compra)
        {
            if (compra == null || !compra.Detalles.Any())
            {
                return BadRequest(new { mensaje = "La compra debe tener al menos un detalle." });
            }

            try
            {
                // int? idUsuario = HttpContext.Session.GetInt32("UsuarioId");
                // if (idUsuario is null) return BadRequest();
                // short id = (short)idUsuario;
                // no llee los ids
                short id = 1;
                int idNuevaCompra = await _compraService.CreateAsync(compra, id);
                return CreatedAtAction(nameof(GetById), new { id = idNuevaCompra }, idNuevaCompra);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno al procesar la compra.", error = ex.Message });
            }
        }
    }
}