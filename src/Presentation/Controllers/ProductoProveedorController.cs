using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using src.Contracts;
using src.Core.Services.Interfaces;
using src.Presentation.ViewModels.ProductoVM;
using Microsoft.AspNetCore.Session;
using src.Presentation.Attributes;
using Microsoft.AspNetCore.Http;
using src.Presentation.ViewModels.ImportacionVM;
using src.Repositories.Interfaces;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace src.Presentation.Controllers
{
    public class ProductoProveedorController : Controller
    {
        private readonly IProductoProveedorService _service;
        private readonly IProveedorRepository _proveedorRepository;
        private readonly INovedadesService _novedadesService;

        public ProductoProveedorController(IProductoProveedorService service,IProveedorRepository proveedorRepository,INovedadesService novedadesService)
        {
            _service = service;
            _proveedorRepository = proveedorRepository;
            _novedadesService = novedadesService;
        }

        [HttpGet]
        [AuthorizePermiso("P08_VER_LISTA_COMPRAS")]
        public async Task<IActionResult> Index()
        {
            var lista = await _service.GetAllParaListadoAsync();
            var cantidadNovedades = await _novedadesService.ObtenerCantidadNovedadesPendientes();
            ViewBag.TotalNovedades = cantidadNovedades; 
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

      
        [HttpPost]
        public async Task ProcesarStream(
            [FromForm] ConfigurarCargaViewModel model,
            CancellationToken cancellationToken)
        {
            // 1. Configurar la respuesta para Streaming de Texto
            Response.ContentType = "application/x-ndjson"; // Newline Delimited JSON
            Response.StatusCode = 200;

            try
            {
                // 2. Validaciones Manuales
                // Si fallan, escribimos un JSON de error y cortamos
                if (model.ArchivoExcel == null || model.ArchivoExcel.Length == 0)
                    throw new ArgumentException("El archivo es obligatorio.");

                if (model.IdProveedor <= 0)
                    throw new ArgumentException("El proveedor es obligatorio.");

                var mapaColumnas = new ImportacionColumnaMap
                {
                    CodigosBarrasExternosIndex = model.ColumnaCodigoBarra,
                    NombreSugeridoIndex = model.ColumnaNombre,
                    PrecioIndex = model.ColumnaPrecio,
                    StockIndex = model.ColumnaStock
                };

                // 3. Procesamiento
                // Usamos OpenReadStream para no copiar todo a memoria (más eficiente)
                using var stream = model.ArchivoExcel.OpenReadStream();

                var opcionesJson = new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
                };

                await foreach (var accion in _service.ProcesarListaDePreciosAsync(
                    stream,
                    model.IdProveedor,
                    mapaColumnas,
                    model.ContieneEncabezado,
                    cancellationToken))
                {
                    // 4. Escribir cada resultado como una línea JSON independiente
                    var jsonLinea = System.Text.Json.JsonSerializer.Serialize(accion, opcionesJson);
                    
                    // Escribimos la línea + salto de línea
                    await Response.WriteAsync(jsonLinea + "\n", cancellationToken);
                    
                    // Forzamos el envío al navegador para que la barra de progreso se mueva
                    await Response.Body.FlushAsync(cancellationToken);
                }
            }
            catch (Exception ex)
            {
                // Si ocurre un error EN MEDIO del proceso, necesitamos avisar al front
                // Como ya mandamos status 200, mandamos un JSON especial de error
                var errorJson = System.Text.Json.JsonSerializer.Serialize(new 
                { 
                    error = true, 
                    mensaje = ex.Message 
                });
                await Response.WriteAsync(errorJson + "\n", cancellationToken);
            }
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

        [HttpGet]
        public async Task<IActionResult> Configurar(short idProveedor, string razonSocial)
        {
            var proveedores = await _proveedorRepository.GetAllProveedorAsync();
            var proveedoresActivos =  proveedores.Where(p => p.Activo == true).ToList();
            var model = new ConfigurarCargaViewModel
            {
                ProveedoresDisponibles = proveedoresActivos.Select(p => new SelectListItem
                {
                    Value = p.IdProveedor.ToString(),
                    Text = p.RazonSocial
                }),
                ColumnaCodigoBarra = -1, 
                ColumnaNombre = -1,      
                ColumnaPrecio = -1,      
                ColumnaStock = -1     
            };
            return View(model);
        }

    }
}



  // [HttpPost]
        // // Ahora devolvemos Task<IActionResult> (la forma estándar de ASP.NET Core)
        // public async IAsyncEnumerable<AccionDeFilaCargaAutomatica> ProcesarStream(
        //     [FromForm] ConfigurarCargaViewModel model, 
        //     [EnumeratorCancellation] CancellationToken cancellationToken)
        // {
        //     if (model.ArchivoExcel == null || model.ArchivoExcel.Length == 0) 
        //         throw new ArgumentException("El archivo es obligatorio.");
            
        //     if (model.IdProveedor <= 0)
        //         throw new ArgumentException("El proveedor es obligatorio.");

        //     var mapaColumnas = new ImportacionColumnaMap
        //     {
        //         CodigosBarrasExternosIndex = model.ColumnaCodigoBarra,
        //         NombreSugeridoIndex = model.ColumnaNombre,
        //         PrecioIndex = model.ColumnaPrecio,
        //         StockIndex = model.ColumnaStock
        //     };

        //     // 2. Copia del Stream
        //     await using var memoryStream = new MemoryStream();
        //     await model.ArchivoExcel.CopyToAsync(memoryStream, cancellationToken);
        //     memoryStream.Position = 0;

        //     // 3. STREAMING PURO (Aprovechamos la lógica de tu compañero)
        //     // Al hacer yield return aquí, cada objeto se envía al JS en cuanto se procesa.
        //     // Si el JS cancela, el 'cancellationToken' de aquí se cancela y se corta el servicio.
        //     await foreach (var accion in _service.ProcesarListaDePreciosAsync(
        //         memoryStream, 
        //         model.IdProveedor, 
        //         mapaColumnas, 
        //         model.ContieneEncabezado, 
        //         cancellationToken))
        //     {
        //         yield return accion;
        //     }
        // }