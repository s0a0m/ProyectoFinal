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


        [HttpGet]
        public async Task<IActionResult> ActualizarProveedor(int idProv)
        {
           
            Dom.Proveedor? prov = await _repoProv.GetProveedorById(idProv);
            if (prov == null)
            {
                // Si el proveedor no existe, redirigir a la lista o mostrar error
                TempData["Error"] = $"Proveedor con ID {idProv} no encontrado.";
                return RedirectToAction("ListarProveedores");
            }

            IEnumerable<Dom.Provincia> listaProvEnumerable = await _repoProv.GetAllProvinciaAsync();
            var listaProv = listaProvEnumerable.ToList(); 

            var viewModel = new ActualizarProveedorViewModel(prov, listaProv);

            return View("ActualizarProveedor", viewModel); 
        }
        
        [HttpPost]
        public async Task<IActionResult> ActualizarProveedor([FromForm] ActualizarProveedorViewModel proveedorVM)
        {
            
            if (!ModelState.IsValid)
            {
                // Si la validación falla, recargar la lista de provincias
                IEnumerable<Dom.Provincia> listaProvinciasError = await _repoProv.GetAllProvinciaAsync();
                proveedorVM.Direccion.ListaProvincias = listaProvinciasError.ToList();
                return View("ActualizarProveedor", proveedorVM);
            }

           
            Dom.Proveedor? proveedorExistente = await _repoProv.GetProveedorById(proveedorVM.IdProveedor);
            if (proveedorExistente == null)
            {
                // Manejar el caso raro donde el proveedor fue eliminado mientras se editaba
                ModelState.AddModelError(string.Empty, "El proveedor que intenta actualizar ya no existe.");
                IEnumerable<Dom.Provincia> listaProvinciasError = await _repoProv.GetAllProvinciaAsync();
                proveedorVM.Direccion.ListaProvincias = listaProvinciasError.ToList();
                return View("ActualizarProveedor", proveedorVM);
            }

            // 3. Mapear los cambios desde el ViewModel (proveedorVM) a la entidad existente (proveedorExistente)
            proveedorExistente.Cuit = proveedorVM.Cuit.Trim();
            proveedorExistente.RazonSocial = proveedorVM.RazonSocial.Trim();
            proveedorExistente.Telefono = proveedorVM.Telefono.Trim();
            proveedorExistente.Correo = proveedorVM.Correo.Trim();
            proveedorExistente.PersonaResponsable = proveedorVM.PersonaResponsable.Trim();
            proveedorExistente.Saldo = proveedorVM.Saldo;
            proveedorExistente.Direccion.Calle = proveedorVM.Direccion.calle.Trim();
            proveedorExistente.Direccion.Numero = proveedorVM.Direccion.numero;
            proveedorExistente.Direccion.Piso = proveedorVM.Direccion.piso;
            proveedorExistente.Direccion.Comentario = proveedorVM.Direccion.comentario?.Trim();


             IEnumerable<Dom.Provincia> listaProvincias = await _repoProv.GetAllProvinciaAsync();
           
             Dom.Provincia? provinciaSeleccionada = listaProvincias
                 .FirstOrDefault(p => p.IdProvincia == proveedorVM.Direccion.provincia.Id_provincia);
             if(provinciaSeleccionada != null) {
                  proveedorExistente.Direccion.Prov = provinciaSeleccionada;
             } else {
                 // Manejar error si la provincia seleccionada no es válida (aunque la validación debería prevenir esto)
                 ModelState.AddModelError("Direccion.provincia.Id_provincia", "La provincia seleccionada no es válida.");
                 proveedorVM.Direccion.ListaProvincias = listaProvincias.ToList();
                 return View("ActualizarProveedor", proveedorVM);
             }

            if (proveedorVM.CondicionPago.Tipo == "Cuota")
            {
                // Si ya era Cuota, actualiza; si era Contado, reemplaza
                var cuota = (proveedorExistente.Condicion as Dom.Cuota) ?? new Dom.Cuota();
                cuota.DiasPago = proveedorVM.CondicionPago.DiasPago;
                cuota.Cuotas = proveedorVM.CondicionPago.NumeroCuotas;
                cuota.InteresPorcentual = proveedorVM.CondicionPago.InteresPorcentual;
                proveedorExistente.Condicion = cuota; // Asigna la instancia (nueva o actualizada)
            }
            else if (proveedorVM.CondicionPago.Tipo == "Contado")
            {
                // Si ya era Contado, actualiza; si era Cuota, reemplaza
                var contado = (proveedorExistente.Condicion as Dom.Contado) ?? new Dom.Contado();
                contado.DiasPago = proveedorVM.CondicionPago.DiasPago;
                proveedorExistente.Condicion = contado; // Asigna la instancia (nueva o actualizada)
            }
            else
            {
                 // Tipo inválido? Manejar error.
                 ModelState.AddModelError("CondicionPago.Tipo", "El tipo de condición de pago no es válido.");
                 proveedorVM.Direccion.ListaProvincias = listaProvincias.ToList(); // Necesario recargar
                 return View("ActualizarProveedor", proveedorVM);
            }

            // 4. Llamar al Update del Repositorio con la entidad del Dominio actualizada
            await _repoProv.UpdateAsync(proveedorExistente);

            TempData["realizado"] = "El Proveedor fue actualizado con éxito."; // Mensaje más específico
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