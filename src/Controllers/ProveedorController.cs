using System.Diagnostics;
using src.Repositories.Interfaces;
using src.Models;
using Microsoft.AspNetCore.Mvc;
using src.ViewModels;

namespace src.Controllers;

public class ProveedorController : Controller
{
    private readonly IProveedorRepository _repoProv;

    public ProveedorController(IProveedorRepository repoProv)
    {
        _repoProv = repoProv;
    }

    [HttpGet]
    public IActionResult VerProveedor(int idProv)
    {
        return View(_repoProv.GetProvByIdAsync(idProv));
    }
    [HttpGet]
    public IActionResult ListarProveedores()
    {
        var ListarProveedores = _repoProv.GetAllProvAsync();
        return View(ListarProveedores);
    }

    [HttpGet]
    public async Task<IActionResult> CrearProveedor()
    {
        List<Provincia> ListaProv = await _repoProv.GetAllProvinciasAsync();
        return View(ListaProv);
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
    public async Task<IActionResult>ActualizarProveedor(int idProv)
    {
        List<Provincia> ListaProv = await _repoProv.GetAllProvinciasAsync();
        Proveedor prov = await _repoProv.GetProvByIdAsync(idProv);
        return View();
    }


    [HttpPost]
    public async Task<IActionResult> ActualizarProveedor(Proveedor prov)
    {
        if (!ModelState.IsValid)
        {
            return View(prov);
        }

        _repoProv.UpdateAsync(prov);
        TempData["realizado"] = "El usuario fue Actualizado con exito.";
        return RedirectToAction("ListarProveedores");

    }


    [HttpGet]
    public IActionResult EliminarProveedor(int idProv)
    {
        _repoProv.DeleteAsync(idProv);
        TempData["realizado"] = "El usuario fue Eliminado con exito.";
        return RedirectToAction("ListarProveedores");
    }


}