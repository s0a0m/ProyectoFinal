using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using src.Contracts;
using src.Core.Services.Interfaces;
using src.Presentation.ViewModels.ProductoVM;
using Microsoft.AspNetCore.Session;
using src.Presentation.Attributes;
using Microsoft.AspNetCore.Http;
namespace src.Presentation.Controllers
{
    public class ProductoProveedorController : Controller
    {
        private readonly IProductoProveedorService _service;

        public ProductoProveedorController(IProductoProveedorService service)
        {
            _service = service;
        }

        [HttpGet]
        [AuthorizePermiso("P08_VER_LISTA_COMPRAS")]
        public async Task<IActionResult> Index()
        {
            var lista = await _service.GetAllParaListadoAsync();
            return View(lista);
        }

        [HttpGet]
        [AuthorizePermiso("P07_GESTION_COMPRAS")]
        public async Task<IActionResult> Create()
        {
            var vm = await _service.PrepararCrearViewModelAsync();
            return View(vm);
        }

        // POST: ProductoProveedor/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermiso("P07_GESTION_COMPRAS")]
        public async Task<IActionResult> Create(CrearProductoProveedorViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                await _service.RepoblarViewModelAsync(vm);
                return View(vm);
            }

            try
            {
                await _service.CreateAsync(vm);
                TempData["Success"] = "Relación creada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                // Error de negocio (ej. relación ya existente)
                ModelState.AddModelError("", ex.Message);
                await _service.RepoblarViewModelAsync(vm);
                return View(vm);
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
                await _service.RepoblarViewModelAsync(vm);
                return View(vm);
            }
            catch (Exception ex)
            {
                var mensajeError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;

                ModelState.AddModelError("", $"ERROR TÉCNICO: {mensajeError}");
                await _service.RepoblarViewModelAsync(vm);
                return View(vm);
            }
        }

       
        [HttpGet]
        [AuthorizePermiso("P07_GESTION_COMPRAS")]
        public async Task<IActionResult> Edit(int idProducto, int idProveedor)
        {
            try
            {
                var vm = await _service.PrepararActualizarViewModelAsync(idProducto, idProveedor);
                return View(vm);
            }
            catch (KeyNotFoundException)
            {
                TempData["Error"] = "La relación solicitada no existe.";
                return RedirectToAction(nameof(Index));
            }
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermiso("P07_GESTION_COMPRAS")]
        public async Task<IActionResult> Edit(ActualizarProductoProveedorViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                // En Edit normalmente no hay listas que repoblar salvo que permitieras cambiar IDs
                return View(vm);
            }

            try
            {
                await _service.UpdateAsync(vm);
                TempData["Success"] = "Relación actualizada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Error al actualizar.");
                return View(vm);
            }
        }

        [HttpPost("upload-lista-precios")]
        // Ahora devolvemos Task<IActionResult> (la forma estándar de ASP.NET Core)
        public IActionResult SubirListaDePrecios(
            IFormFile archivo,
            [FromQuery] short idProveedor,
            [FromQuery] ImportacionColumnaMap mapaColumnas,
            CancellationToken cancellationToken,
            bool contieneEncabezado = true
            )
        {
            // Llamamos al método auxiliar que maneja la asincronía y el yield return
            var resultadosStream = EjecutarCargaStream(archivo, idProveedor, mapaColumnas, contieneEncabezado, cancellationToken);

            // Devolvemos el IAsyncEnumerable envuelto en Ok() para que el framework lo serialice
            return Ok(resultadosStream);
        }

        // MÉTODO AUXILIAR que combina await con yield return
        private async IAsyncEnumerable<AccionDeFilaCargaAutomatica> EjecutarCargaStream(
            IFormFile archivo,
            short idProveedor,
            ImportacionColumnaMap mapaColumnas, bool contieneEncabezado,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {

            if (archivo == null || archivo.Length == 0)
            {
                throw new ArgumentException("No se ha proporcionado ningún archivo o el archivo está vacío.");
            }

            const long TAMAÑO_MÁXIMO_BYTES = 5 * 1024 * 1024;
            if (archivo.Length > TAMAÑO_MÁXIMO_BYTES)
            {
                throw new ArgumentException("El archivo excede el tamaño máximo permitido de 5 MB.");
            }

            if (archivo.ContentType != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" &&
                archivo.ContentType != "application/vnd.ms-excel")
            {
                throw new ArgumentException("El archivo debe ser un documento de Excel (.xls o .xlsx).");
            }

            // luego escanear virus con algun pipeline externo 



            // 1. Copiar y preparar el Stream
            await using var memoryStream = new MemoryStream();
            await archivo.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            // 2. Consumir el IAsyncEnumerable del servicio y devolver sus resultados
            await foreach (var accion in _service.ProcesarListaDePreciosAsync(memoryStream, idProveedor, mapaColumnas, contieneEncabezado, cancellationToken))
            {
                yield return accion; // Pasamos el resultado al cliente inmediatamente
            }
        }

    }
}