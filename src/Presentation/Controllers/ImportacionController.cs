using System.Text.Json; // Necesario para JsonSerializer
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using src.Contracts;
using src.Core.Services.Interfaces;
using src.Presentation.ViewModels.ImportacionVM;
using src.Repositories.Interfaces;

namespace src.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImportacionController : Controller
    {
        private readonly IImportacionService _importacionService;
        private readonly IProveedorRepository _proveedorRepository;
        private readonly ILogger<ImportacionController> _logger;

        public ImportacionController(
            IImportacionService importacionService,
            IProveedorRepository proveedorRepository,
            ILogger<ImportacionController> logger)
        {
            _importacionService = importacionService;
            _proveedorRepository = proveedorRepository;
            _logger = logger;
        }
        [HttpGet("nueva")]
        public async Task<IActionResult> NuevaCarga()
        {
            // Necesitas inyectar IProveedorRepository o similar
            var proveedores = await _proveedorRepository.GetAllProveedorAsync();

            var viewModel = new ConfigurarCargaViewModel
            {
                ProveedoresDisponibles = proveedores.Select(p => new SelectListItem
                {
                    Value = p.IdProveedor.ToString(),
                    Text = p.RazonSocial
                })
            };

            return View(viewModel);
        }

        [HttpPost("lista-precios")]
        [DisableRequestSizeLimit]
        public async Task CargarListaPrecios(
            [FromForm] ConfigurarCargaViewModel input,
            CancellationToken cancellationToken)
        {
            if (input.ArchivoExcel == null || input.ArchivoExcel.Length == 0)
            {
                Response.StatusCode = 400;
                await Response.WriteAsync("No se ha subido ningún archivo.", cancellationToken);
                return;
            }

            if (!ModelState.IsValid)
            {
                Response.StatusCode = 400;
                await Response.WriteAsync("Datos del formulario inválidos.", cancellationToken);
                return;
            }

            var extension = Path.GetExtension(input.ArchivoExcel.FileName).ToLower();
            if (extension != ".xlsx" && extension != ".xls")
            {
                Response.StatusCode = 400;
                await Response.WriteAsync("El archivo debe ser un documento de Excel (.xls o .xlsx).", cancellationToken);
                return;
            }

            _logger.LogInformation("Iniciando carga masiva para proveedor {IdProveedor}", input.IdProveedor);

            var mapaColumnas = new ImportacionColumnaMap
            {
                CodigosBarrasExternosIndex = input.ColumnaCodigoBarra,
                NombreSugeridoIndex = input.ColumnaNombre,
                PrecioIndex = input.ColumnaPrecio,
                StockIndex = input.ColumnaStock
            };

            Response.ContentType = "application/x-ndjson";

            using var stream = input.ArchivoExcel.OpenReadStream();
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping // Opcional: para que no escape caracteres raros
            };
            await foreach (var resultado in _importacionService.ProcesarListaDePreciosAsync(
                stream,
                input.IdProveedor,
                mapaColumnas,
                input.ContieneEncabezado,
                cancellationToken))
            {
                var jsonLine = JsonSerializer.Serialize(resultado, jsonOptions);
                await Response.WriteAsync(jsonLine + "\n", cancellationToken);

                await Response.Body.FlushAsync(cancellationToken);
            }

            _logger.LogInformation("Finalizada carga masiva para proveedor {IdProveedor}", input.IdProveedor);
        }
    }
}