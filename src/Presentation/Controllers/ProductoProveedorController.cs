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
    public class ProductoProveedorController : BaseController
    {
        private readonly IProductoProveedorService _service;
        private readonly IProveedorRepository _proveedorRepository;
        private readonly INovedadesService _novedadesService;

        public ProductoProveedorController(IProductoProveedorService service, IProveedorRepository proveedorRepository, INovedadesService novedadesService)
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

            // Recibimos el objeto ServiceResult
            var result = await _service.CreateAsync(vm);

            if (!result.Success)
            {
                // El BaseController se encarga de repartir los errores en los labels rojos
                MapServiceErrors(result);
                await _service.RepoblarViewModelAsync(vm);
                return View(vm);
            }

            SetSuccessMessage(result.Message);
            return RedirectToAction(nameof(Index));

            // try
            // {
            //     await _service.CreateAsync(vm);
            //     TempData["Success"] = "Relación creada correctamente.";
            //     return RedirectToAction(nameof(Index));
            // }
            // catch (InvalidOperationException ex)
            // {
            //     // Error de negocio (ej. relación ya existente)
            //     ModelState.AddModelError("", ex.Message);
            //     await _service.RepoblarViewModelAsync(vm);
            //     return View(vm);
            // }
            // catch (ArgumentException ex)
            // {
            //     ModelState.AddModelError("", ex.Message);
            //     await _service.RepoblarViewModelAsync(vm);
            //     return View(vm);
            // }
            // catch (Exception ex)
            // {
            //     var mensajeError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;

            //     ModelState.AddModelError("", $"ERROR TÉCNICO: {mensajeError}");
            //     await _service.RepoblarViewModelAsync(vm);
            //     return View(vm);
            // }
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


        [HttpGet]
        public async Task<IActionResult> Configurar(short idProveedor, string razonSocial)
        {
            var proveedores = await _proveedorRepository.GetAllProveedorAsync();
            var proveedoresActivos = proveedores.Where(p => p.Activo == true).ToList();
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