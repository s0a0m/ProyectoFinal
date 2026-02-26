using Microsoft.AspNetCore.Mvc;
using src.Core.Services.Interfaces;
using src.Presentation.ViewModels.NovedadesVM;
using src.Repositories.Interfaces;
namespace src.Presentation.Controllers;

using Microsoft.AspNetCore.Session;
using Microsoft.AspNetCore.Http;
using src.Presentation.Attributes;
using Microsoft.AspNetCore.Mvc.Rendering;

public class NovedadesController : Controller
{
    private readonly INovedadesService _novedadesService;
    private readonly INovedadesRepository _novedadesRepo;
    private readonly IProveedorRepository _proveedorRepo;
    private readonly IProductoRepository _productoRepository;
    public NovedadesController(INovedadesService novedadesService, INovedadesRepository novedadesRepo, IProveedorRepository proveedorRepo, IProductoRepository productoRepository)
    {
        _novedadesService = novedadesService;
        _novedadesRepo = novedadesRepo;
        _proveedorRepo = proveedorRepo;
        _productoRepository = productoRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var novedades = await _novedadesService.GetAllNovedadesPendientes();
        return View(novedades);
    }

    [HttpPost]
    public async Task<IActionResult> CrearNovedades(NovedadesCrearViewModel novedadVM)
    {
        if (!ModelState.IsValid)
        {
            var lista = await _novedadesService.GetAllNovedadesPendientes();
            return View("Index", lista);
        }
        try
        {
            await _novedadesService.CrearNovedadAsync(novedadVM);
            TempData["Success"] = "Novedad creada con éxito";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }


    [HttpGet]
    public async Task<IActionResult> Resolver(int id)
    {
        try
        {
            // Reutilizamos tu lógica para obtener datos, pero ahora mapeamos al nuevo VM
            var novedad = await _novedadesRepo.GetByIdAsync(id);
            if (novedad == null) return NotFound();

            var proveedor = await _proveedorRepo.GetProveedorById(novedad.IdProveedor);

            var model = new ResolverNovedadViewModel
            {
                IdNovedad = novedad.IdNovedad,
                RazonSocialProveedor = proveedor?.RazonSocial ?? "Desconocido",
                CodigoBarraNovedad = novedad.CodigoBarraExterno,
                NombreSugerido = novedad.NombreSugerido,
                PrecioSugerido = novedad.PrecioSugerido,

                // Pre-cargamos los inputs con los datos del Excel (si existen)
                PrecioFinal = novedad.PrecioSugerido, // Si es 0, el usuario deberá editarlo
                StockFinal = 0
            };

            // Cargar lista de productos para el Select (Dropdown)
            // Recomendación: Si tienes 5000 productos, usa Select2 con búsqueda AJAX en el futuro.
            // Por ahora cargamos todos como pediste.
            model.ProductosDisponibles = await ObtenerListaProductos();

            return View(model);
        }
        catch (Exception) { return NotFound(); }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Resolver(ResolverNovedadViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.ProductosDisponibles = await ObtenerListaProductos();

            // Asegurarnos de que los datos de solo lectura no se pierdan
            // (Aunque deberían venir por los input hidden, a veces es mejor recargarlos si es posible
            // o confiar en que la vista los mandó bien).

            return View(model);
        }

        try
        {
            await _novedadesService.AceptarNovedadAsync(model);
            TempData["Success"] = "Novedad resuelta correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            model.ProductosDisponibles = await ObtenerListaProductos();
            return View(model);
        }
    }

    // Acción separada para el botón de rechazar
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Rechazar(int idNovedad)
    {
        await _novedadesService.RechazarNovedadAsync(idNovedad);
        TempData["Success"] = "Novedad rechazada y archivada.";
        return RedirectToAction(nameof(Index));
    }



    private async Task<IEnumerable<SelectListItem>> ObtenerListaProductos()
    {
        var productos = await _productoRepository.GetAllAsync();
        return productos.Select(p => new SelectListItem
        {
            Value = p.IdProducto.ToString(),
            // Usamos string interpolation segura (?. y ?? para nulos)
            Text = $"{p.Nombre}"
        });
    }



}