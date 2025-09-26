using System.Diagnostics;
using src.Repositories.Interfaces;
using src.Models;
using Microsoft.AspNetCore.Mvc;
using src.ViewModels;

namespace src.Controllers;

[ApiController]
[Route("[controller]")]

public class ProveedorController : Controller
{
    private readonly IProveedorRepository _repoProv;

    public ProveedorController(IProveedorRepository repoProv)
    {
        _repoProv = repoProv;
    }

    [HttpGet("Ver/{idProv}")]
    public IActionResult VerProveedor(int idProv)
    {
        return View(_repoProv.GetProvByIdAsync(idProv));
    }
    [HttpGet]
    [ActionName("Index")]
    public async Task<IActionResult> ListarProveedores()
    {
        var ListarProveedores = await _repoProv.GetAllProvAsync();
        return View("ListarProveedores", ListarProveedores);
    }

    [HttpGet]
    public async Task<IActionResult> CrearProveedor()
    {

        List<Provincia> listaProvincias = await _repoProv.GetAllProvinciasAsync();
        var viewModel = new CrearProveedorViewModel
        {
            Direccion = new DireccionViewModel
            {
                ListaProvincias = listaProvincias
            }
        };
        return View(viewModel);
    }


    [HttpPost]
    public async Task<IActionResult> CrearProveedor(CrearProveedorViewModel proveedorVM)
    {
        if (!ModelState.IsValid)
        {
            return View(proveedorVM);
        }
        Proveedor proveedor = new(proveedorVM);
        await _repoProv.CreateAsync(proveedor);
        TempData["realizado"] = "El Proveedor fue creado con exito.";
        return RedirectToAction("ListarProveedores");
    }

    [HttpGet]
    public async Task<IActionResult> ActualizarProveedor(int idProv)
    {
        List<Provincia> ListaProv = await _repoProv.GetAllProvinciasAsync();
        Proveedor prov = await _repoProv.GetProvByIdAsync(idProv);
        return View();
    }

    [HttpPost]
    public IActionResult ActualizarProveedor(Proveedor prov)
    {
        if (!ModelState.IsValid)
        {
            return View(prov);
        }

        _repoProv.UpdateAsync(prov);
        TempData["realizado"] = "El usuario fue Actualizado con exito.";
        return RedirectToAction("ListarProveedores");
    }


    [HttpGet("Eliminar/{idProv}")]
    public IActionResult EliminarProveedor(int idProv)
    {
        _repoProv.DeleteAsync(idProv);
        TempData["realizado"] = "El usuario fue Eliminado con exito.";
        return RedirectToAction("ListarProveedores");
    }
}