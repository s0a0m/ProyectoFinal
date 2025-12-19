using System.Diagnostics;
using src.Repositories.Interfaces;
using EF = src.Models.CodeFirst;
using Dom = src.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using src.ViewModels;
using src.Core.Services.Interfaces;
using Microsoft.AspNetCore.Session;
using Microsoft.AspNetCore.Http;
using src.Presentation.Attributes;

namespace src.Controllers;


public class ProveedorController : Controller
{
    private readonly ICommonDataService _commonDataService;
    private readonly IProveedorService _provService;

    public ProveedorController(ICommonDataService commonDataService, IProveedorService provService)
    {
        _commonDataService = commonDataService;
        _provService = provService;
    }

    [HttpGet]
    [AuthorizePermiso("P06_VER_LISTA_PROVEEDORES")]
    public async Task<IActionResult> VerProveedor(int idProv)
    {
        try
        {
            Dom.Proveedor proveedor = await _provService.GetProveedorByIdAsync(idProv);
            return View(proveedor);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet]
    [AuthorizePermiso("P06_VER_LISTA_PROVEEDORES")]
    public async Task<IActionResult> ListarProveedores()
    {
        var listarProveedores = await _provService.GetActiveProveedoresAsync();
        return View("ListarProveedores", listarProveedores);
    }

    [HttpGet]
    [AuthorizePermiso("P05_ABM_PROVEEDORES")]
    public async Task<IActionResult> CrearProveedor()
    {
        return View(await PrepareCrearProveedorViewModel(new CrearProveedorViewModel()));
    }


    [HttpPost]
    [AuthorizePermiso("P05_ABM_PROVEEDORES")]
    public async Task<IActionResult> CrearProveedor([FromForm] CrearProveedorViewModel proveedorVM)
    {
        if (!ModelState.IsValid)
        {
            return View(await PrepareCrearProveedorViewModel(proveedorVM));
        }

        try
        {
            await _provService.CreateProveedorAsync(proveedorVM);
            TempData["realizado"] = "El Proveedor fue creado con exito.";
            return RedirectToAction("ListarProveedores");
        }
        catch (ArgumentException ex) // Para errores de validación de negocio (ej. CUIT duplicado)
        {
            ModelState.AddModelError(ex.ParamName ?? string.Empty, ex.Message);

            return View(await PrepareCrearProveedorViewModel(proveedorVM));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [AuthorizePermiso("P05_ABM_PROVEEDORES")]
    public async Task<IActionResult> EliminarProveedor(int idProv)
    {
        try
        {
            // Llamamos al nuevo método estandarizado que incluye la lógica en cascada
            await _provService.DeleteAsync(idProv);

            TempData["realizado"] = "El Proveedor fue desactivado con éxito (y sus productos asociados se ocultaron).";
        }
        catch (KeyNotFoundException)
        {
            TempData["error"] = $"Error: El Proveedor con ID {idProv} no fue encontrado.";
        }
        catch (Exception ex)
        {
            TempData["error"] = $"Ocurrió un error inesperado: {ex.Message}";
        }

        return RedirectToAction("ListarProveedores");
    }

    // NUEVO MÉTODO: Reactivar
    [HttpPost]
    [ValidateAntiForgeryToken]
    [AuthorizePermiso("P05_ABM_PROVEEDORES")]
    public async Task<IActionResult> ReactivarProveedor(int idProv)
    {
        try
        {
            // Llama al servicio que reactiva el proveedor y
            // chequea qué productos pueden volver a activarse
            await _provService.ReactivateAsync(idProv);

            TempData["realizado"] = "El Proveedor fue reactivado correctamente.";
        }
        catch (KeyNotFoundException)
        {
            TempData["error"] = $"Error: El Proveedor con ID {idProv} no fue encontrado.";
        }
        catch (Exception ex)
        {
            TempData["error"] = $"Ocurrió un error al reactivar: {ex.Message}";
        }

        return RedirectToAction("ListarProveedores");
    }


    [HttpGet]
    [AuthorizePermiso("P05_ABM_PROVEEDORES")]
    public async Task<IActionResult> ActualizarProveedor(int idProv)
    {
        Dom.Proveedor proveedor;

        try
        {
            proveedor = await _provService.GetProveedorByIdAsync(idProv);
        }
        catch (KeyNotFoundException)
        {
            TempData["Error"] = $"Proveedor con ID {idProv} no encontrado.";
            return RedirectToAction("ListarProveedores");
        }

        var viewModel = new ActualizarProveedorViewModel(proveedor);

        return View("ActualizarProveedor", await PrepareActualizarProveedorViewModel(viewModel));
    }

    [HttpPost]
    [AuthorizePermiso("P05_ABM_PROVEEDORES")]
    public async Task<IActionResult> ActualizarProveedor([FromForm] ActualizarProveedorViewModel proveedorVM)
    {
        if (!ModelState.IsValid)
        {
            return View("ActualizarProveedor", await PrepareActualizarProveedorViewModel(proveedorVM));
        }
        try
        {
            await _provService.UpdateProveedorAsync(proveedorVM);
            TempData["realizado"] = "El Proveedor fue actualizado con éxito.";
            return RedirectToAction("ListarProveedores");
        }
        catch (KeyNotFoundException)
        {
            TempData["Error"] = "El proveedor que intenta actualizar ya no existe.";
            return RedirectToAction("ListarProveedores");
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(ex.ParamName ?? string.Empty, ex.Message);

            return View("ActualizarProveedor", await PrepareActualizarProveedorViewModel(proveedorVM));
        }
    }
    private async Task<CrearProveedorViewModel> PrepareCrearProveedorViewModel(CrearProveedorViewModel model)
    {
        IEnumerable<Dom.Provincia> listaProvincias = await _commonDataService.GetAllProvinciasAsync();
        model.Direccion.ListaProvincias = listaProvincias.ToList();
        return model;
    }
    private async Task<ActualizarProveedorViewModel> PrepareActualizarProveedorViewModel(ActualizarProveedorViewModel model)
    {
        IEnumerable<Dom.Provincia> listaProvincias = await _commonDataService.GetAllProvinciasAsync();
        model.Direccion.ListaProvincias = listaProvincias.ToList();
        return model;
    }
}