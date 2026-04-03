using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Session;
using src.Core.Services.Interfaces;
using src.Presentation.Attributes;
using src.Presentation.Mappers;
using src.Repositories.Interfaces;
using src.ViewModels;
using Dom = src.Models.Domain;
using EF = src.Models.CodeFirst;

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
        var provincias = await _commonDataService.GetAllProvinciasAsync();
        var vm = new CrearProveedorViewModel().PrepareWithProvincias(provincias);
        return View(vm);
    }

    [HttpPost]
    [AuthorizePermiso("P05_ABM_PROVEEDORES")]
    public async Task<IActionResult> CrearProveedor([FromForm] CrearProveedorViewModel proveedorVM)
    {
        if (!ModelState.IsValid)
        {
            var provincias = await _commonDataService.GetAllProvinciasAsync();
            return View(proveedorVM.PrepareWithProvincias(provincias));
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
            var provincias = await _commonDataService.GetAllProvinciasAsync();
            return View(proveedorVM.PrepareWithProvincias(provincias));
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

            TempData["realizado"] =
                "El Proveedor fue desactivado con éxito (y sus productos asociados se ocultaron).";
        }
        catch (KeyNotFoundException)
        {
            TempData["error"] = $"Error: El Proveedor no fue encontrado.";
        }
        catch (Exception ex)
        {
            TempData["error"] = $"Ocurrió un error inesperado: {ex.Message}";
        }

        return RedirectToAction("ListarProveedores");
    }

    [HttpGet]
    [AuthorizePermiso("P09_GESTOR_CC")]
    public async Task<IActionResult> CuentaCorriente(short id)
    {
        var data = await _provService.ObtenerCuentaCorrienteAsync(id);
        if (data == null)
        {
            TempData["Error"] = "Proveedor no encontrado.";
            return RedirectToAction("ListarProveedores");
        }

        return View(data.ToCuentaCorrienteVM());
    }

    // [HttpGet]
    // [AuthorizePermiso("P09_GESTOR_CC")]
    // public async Task<IActionResult> CuentaCorriente(short id)
    // {
    //     try
    //     {
    //         var vm = await _provService.ObtenerCuentaCorrienteAsync(id);
    //         if (vm == null)
    //         {
    //             TempData["Error"] = "Proveedor no encontrado.";
    //             return RedirectToAction("Index");
    //         }
    //         return View(vm);
    //     }
    //      catch (Exception ex)
    //     {
    //         TempData["error"] = $"Ocurrió un error inesperado: {ex.Message}";
    //         return RedirectToAction("Index");
    //     }
    // }

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
        try
        {
            var proveedor = await _provService.GetProveedorByIdAsync(idProv);
            var provincias = await _commonDataService.GetAllProvinciasAsync();
            var viewModel = proveedor.ToActualizarVM().PrepareWithProvincias(provincias);
            
            return View("ActualizarProveedor", viewModel);
        }
        catch (KeyNotFoundException)
        {
            TempData["Error"] = $"Proveedor con ID {idProv} no encontrado.";
            return RedirectToAction("ListarProveedores");
        }
    }

    [HttpPost]
    [AuthorizePermiso("P05_ABM_PROVEEDORES")]
    public async Task<IActionResult> ActualizarProveedor(
        [FromForm] ActualizarProveedorViewModel proveedorVM
    )
    {
        if (!ModelState.IsValid)
        {
            var provincias = await _commonDataService.GetAllProvinciasAsync();
            return View("ActualizarProveedor", proveedorVM.PrepareWithProvincias(provincias));
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
            var provincias = await _commonDataService.GetAllProvinciasAsync();
            return View("ActualizarProveedor", proveedorVM.PrepareWithProvincias(provincias));
        }
    }
}
