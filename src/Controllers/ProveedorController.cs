using System.Diagnostics;
using src.Repositories.Interfaces;
using EF = src.Models.CodeFirst;
using Dom = src.Models.Domain;
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
    public async Task<IActionResult> VerProveedor(int idProv)
    {
        return View(await _repoProv.GetProveedorById(idProv));
    }

    [HttpGet]
    public async Task<IActionResult> ListarProveedores()
    {
        var ListarProveedores = await _repoProv.GetAllProveedorAsync();
        return View("ListarProveedores", ListarProveedores);
    }

    [HttpGet]
    public async Task<IActionResult> CrearProveedor()
    {

        IEnumerable<Dom.Provincia> listaProvincias = await _repoProv.GetAllProvinciaAsync();
        var ProveedorViewModel = new CrearProveedorViewModel
        {
            Direccion = new DireccionViewModel
            {
                ListaProvincias = listaProvincias.ToList()
            }
        };
        return View(ProveedorViewModel);
    }


    [HttpPost]
    public async Task<IActionResult> CrearProveedor([FromForm] CrearProveedorViewModel proveedorVM)
    {
        if (!ModelState.IsValid)
        {
            IEnumerable<Dom.Provincia> listaProvincias1 = await _repoProv.GetAllProvinciaAsync();
            proveedorVM.Direccion.ListaProvincias = listaProvincias1.ToList();
            return View(proveedorVM);
        }
        IEnumerable<Dom.Provincia> listaProvincias = await _repoProv.GetAllProvinciaAsync();
        proveedorVM.Direccion.ListaProvincias = listaProvincias.ToList();
        Dom.Proveedor proveedor = CrearProveedorViewModel.cargarProveedor(proveedorVM);
        await _repoProv.AddAsync(proveedor);
        TempData["realizado"] = "El Proveedor fue creado con exito.";
        return RedirectToAction("ListarProveedores");
    }

   

    [HttpGet]
    public async Task<IActionResult> EliminarProveedor(int idProv)
    {
        await _repoProv.DeleteAsync(idProv);
        TempData["realizado"] = "El usuario fue Eliminado con exito.";
        return RedirectToAction("ListarProveedores");
    }

/*

     [HttpGet]
    
    public async Task<IActionResult> ActualizarProveedor(int idProv)
    {
         IEnumerable<Dom.Provincia> ListaProv = await _repoProv.GetAllProvinciaAsync();
        Dom.Proveedor prov = await _repoProv.GetProveedorById(idProv);
        var ProveedorViewModel = new CrearProveedorViewModel
        {
            Direccion = new DireccionViewModel
            {
                ListaProvincias = ListaProv.ToList()
            }
        };
        return View(prov);
    }

    [HttpPost]
    public async Task<IActionResult> ActualizarProveedor([FromForm] CrearProveedorViewModel proveedorVM)
    {
        if (!ModelState.IsValid)
        {
            IEnumerable<Dom.Provincia> listaProvincias1 = await _repoProv.GetAllProvinciaAsync();
            proveedorVM.Direccion.ListaProvincias = listaProvincias1.ToList();
            return View(proveedorVM);
        }
         IEnumerable<Dom.Provincia> listaProvincias = await _repoProv.GetAllProvinciaAsync();
        proveedorVM.Direccion.ListaProvincias = listaProvincias.ToList();
        Dom.Proveedor proveedor = CrearProveedorViewModel.cargarProveedor(proveedorVM);
        await _repoProv.UpdateAsync(proveedor);
        TempData["realizado"] = "El usuario fue Actualizado con exito.";
        return RedirectToAction("ListarProveedores");
    }





    [HttpGet("Ver/{idProv}")]
    public async Task<IActionResult> VerProveedor(int idProv)
    {
        return View(await _repoProv.GetProvByIdAsync(idProv));
    }
    [HttpGet("ListarProveedores")]
    public async Task<IActionResult> ListarProveedores()
    {
        var ListarProveedores = await _repoProv.GetAllProvAsync();
        return View("ListarProveedores", ListarProveedores);
    }

    [HttpGet("Crear")]
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


    [HttpPost("Crear")]
    public async Task<IActionResult> CrearProveedor([FromForm] CrearProveedorViewModel proveedorVM)
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

    [HttpGet("Actualizar/{idProv}")]
    public async Task<IActionResult> ActualizarProveedor(int idProv)
    {
        List<Provincia> ListaProv = await _repoProv.GetAllProvinciasAsync();
        Proveedor prov = await _repoProv.GetProvByIdAsync(idProv);
        var viewModel = new CrearProveedorViewModel
        {
            Direccion = new DireccionViewModel
            {
                ListaProvincias = ListaProv
            }
        };
        return View(viewModel);
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
    public async Task<IActionResult> EliminarProveedor(int idProv)
    {
        await _repoProv.DeleteAsync(idProv);
        TempData["realizado"] = "El usuario fue Eliminado con exito.";
        return RedirectToAction("ListarProveedores");
    }
*/
}