using System.Diagnostics;
using src.Repositories.Interfaces;
using src.Models;
using Microsoft.AspNetCore.Mvc;


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
    public IActionResult CrearProveedor()
    {
        return View();
    }


    [HttpPost]
    public IActionResult CrearProveedor(Proveedor pr)
    {
        _repoProv.CreateAsync(pr);
        TempData["realizado"] = "El usuario fue creado con exito.";
        return RedirectToAction("ListarProveedores");
    }

    [HttpGet]
    public IActionResult ActualizarProveedor(int idProv)
    {
        return View(_repoProv.GetProvByIdAsync(idProv));
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


    [HttpGet]
    public IActionResult EliminarProveedor(int idProv)
    {
        _repoProv.DeleteAsync(idProv);
        TempData["realizado"] = "El usuario fue Eliminado con exito.";
        return RedirectToAction("ListarProveedores");
    }

}