// using System.Diagnostics;
// using src.Repositories.Interfaces;
// using src.Models;
// using Microsoft.AspNetCore.Mvc;
// using src.ViewModels;

// namespace src.Controllers;

// [ApiController]
// [Route("[controller]")]

// public class ProveedorController : Controller
// {
//     private readonly IProveedorRepository _repoProv;

//     public ProveedorController(IProveedorRepository repoProv)
//     {
//         _repoProv = repoProv;
//     }

//     [HttpGet("Ver/{idProv}")]
//     public async Task<IActionResult> VerProveedor(int idProv)
//     {
//         return View(await _repoProv.GetProvByIdAsync(idProv));
//     }
//     [HttpGet("ListarProveedores")]
//     public async Task<IActionResult> ListarProveedores()
//     {
//         var ListarProveedores = await _repoProv.GetAllProvAsync();
//         return View("ListarProveedores", ListarProveedores);
//     }

//     [HttpGet("Crear")]
//     public async Task<IActionResult> CrearProveedor()
//     {

//         List<Provincia> listaProvincias = await _repoProv.GetAllProvinciasAsync();
//         var viewModel = new CrearProveedorViewModel
//         {
//             Direccion = new DireccionViewModel
//             {
//                 ListaProvincias = listaProvincias
//             }
//         };
//         return View(viewModel);
//     }


//     [HttpPost("Crear")]
//     public async Task<IActionResult> CrearProveedor([FromForm] CrearProveedorViewModel proveedorVM)
//     {
//         if (!ModelState.IsValid)
//         {
//             return View(proveedorVM);
//         }
//         Proveedor proveedor = new(proveedorVM);
//         await _repoProv.CreateAsync(proveedor);
//         TempData["realizado"] = "El Proveedor fue creado con exito.";
//         return RedirectToAction("ListarProveedores");
//     }

//     [HttpGet("Actualizar/{idProv}")]
//     public async Task<IActionResult> ActualizarProveedor(int idProv)
//     {
//         List<Provincia> ListaProv = await _repoProv.GetAllProvinciasAsync();
//         Proveedor prov = await _repoProv.GetProvByIdAsync(idProv);
//         var viewModel = new CrearProveedorViewModel
//         {
//             Direccion = new DireccionViewModel
//             {
//                 ListaProvincias = ListaProv
//             }
//         };
//         return View(viewModel);
//     }

//     [HttpPost]
//     public IActionResult ActualizarProveedor(Proveedor prov)
//     {
//         if (!ModelState.IsValid)
//         {
//             return View(prov);
//         }

//         _repoProv.UpdateAsync(prov);
//         TempData["realizado"] = "El usuario fue Actualizado con exito.";
//         return RedirectToAction("ListarProveedores");
//     }


//     [HttpGet("Eliminar/{idProv}")]
//     public async Task<IActionResult> EliminarProveedor(int idProv)
//     {
//         await _repoProv.DeleteAsync(idProv);
//         TempData["realizado"] = "El usuario fue Eliminado con exito.";
//         return RedirectToAction("ListarProveedores");
//     }
// }