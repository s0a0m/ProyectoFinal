using Microsoft.AspNetCore.Mvc;
using src.Core.Services.Interfaces;
using src.Presentation.Mappers;
using src.Presentation.ViewModels.FacturaVM;

namespace src.Presentation.Controllers;

public class FacturaController : Controller
{
    private readonly IFacturaService _facturaService;
    private readonly ICompraService _compraService;

    public FacturaController(IFacturaService facturaService, ICompraService compraService)
    {
        _facturaService = facturaService;
        _compraService = compraService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var facturas = await _facturaService.ObtenerTodasAsync();
        return View(facturas.ToListVM());
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var factura = await _facturaService.ObtenerPorIdAsync(id);
        if (factura == null)
            return NotFound();

        return View(factura.ToDetalleVM());
    }

    [HttpGet]
    public async Task<IActionResult> CreateFromCompra(int idCompra)
    {
        var compra = await _compraService.ObtenerPorIdAsync(idCompra);
        if (compra == null)
        {
            TempData["Error"] = "Compra no encontrada";
            return RedirectToAction("Index", "Compra");
        }

        return View(compra.ToCrearVM());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(CrearFacturaViewModel model)
    {
        if (model.TipoCondicion == "Cuota")
        {
            if (!model.CantidadCuotas.HasValue || model.CantidadCuotas < 1)
                ModelState.AddModelError(
                    "CantidadCuotas",
                    "Debe especificar la cantidad de cuotas."
                );
            if (!model.InteresPorcentual.HasValue)
                ModelState.AddModelError("InteresPorcentual", "Debe especificar el interés.");
        }

        if (!ModelState.IsValid)
            return View("CreateFromCompra", model);

        bool existe = await _facturaService.ExisteNumeroFacturaAsync(
            (short)model.IdProveedor,
            model.NumeroFactura
        );
        if (existe)
        {
            ModelState.AddModelError(
                "NumeroFactura",
                "Este número de factura ya existe para este proveedor."
            );
            return View("CreateFromCompra", model);
        }

        var factura = model.ToDomain();
        var resultado = await _facturaService.CrearDesdeCompraAsync(factura, model.IdCompra);

        if (!resultado.Success)
        {
            ModelState.AddModelError("", resultado.Message);
            return View("CreateFromCompra", model);
        }

        TempData["Success"] = resultado.Message;
        return RedirectToAction("Details", new { id = resultado.Data });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var factura = await _facturaService.ObtenerPorIdAsync(id);
        if (factura == null)
        {
            TempData["Error"] = "Factura no encontrada";
            return RedirectToAction("Index");
        }

        return View(factura.ToEditVM());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ActualizarFacturaViewModel model)
    {
        if (model.TipoCondicion == "Cuota")
        {
            if (!model.CantidadCuotas.HasValue || model.CantidadCuotas < 1)
                ModelState.AddModelError(
                    "CantidadCuotas",
                    "Debe especificar la cantidad de cuotas."
                );
            if (!model.InteresPorcentual.HasValue)
                ModelState.AddModelError("InteresPorcentual", "Debe especificar el interés.");
        }

        if (!ModelState.IsValid)
            return View(model);

        var factura = model.ToDomain();
        var resultado = await _facturaService.ActualizarAsync(factura);

        if (!resultado.Success)
        {
            ModelState.AddModelError("", resultado.Message);
            return View(model);
        }

        TempData["Success"] = resultado.Message;
        return RedirectToAction("Details", new { id = model.IdFactura });
    }

    [HttpGet]
    public async Task<IActionResult> DocumentosAsociados(int id)
    {
        var data = await _facturaService.ObtenerDocumentosAsociadosAsync(id);
        if (data == null)
        {
            TempData["Error"] = "Factura no encontrada";
            return RedirectToAction("Index");
        }

        return View(data.ToDocumentosVM());
    }
}
