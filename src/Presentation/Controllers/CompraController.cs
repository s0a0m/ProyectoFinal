using Microsoft.AspNetCore.Mvc;
using src.Core.Services.Interfaces;
using src.Presentation.ViewModels.CompraVM;
using src.Models.Domain;
using src.Models.Common;
using src.Repositories.Interfaces;
using src.Repositories.Interfaces;
using src.Presentation.Attributes;


namespace src.Presentation.Controllers
{
    public class CompraController : Controller
    {
        private readonly ICartService _cartService;
        private readonly ICompraRepository _compraRepo;
        private readonly IUserService _userService;

        public CompraController(ICartService cartService, ICompraRepository compraRepo, IUserService userService)
        {
            _cartService = cartService;
            _compraRepo = compraRepo;
            _userService = userService;
        }

        [HttpGet]
        [AuthorizePermiso("P07_GESTION_COMPRAS")]
        public async Task<IActionResult> Previsualizar(short idProveedor)
        {
            var itemsProveedor = await _cartService.ObtenerItemsPorProveedorAsync(idProveedor);

            if (!itemsProveedor.Any())
            {
                TempData["Error"] = "No hay productos seleccionados para este proveedor.";
                return RedirectToAction("Index", "Carrito");
            }

            var viewModel = new ConfirmarCompraViewModel
            {
                IdProveedor = idProveedor,
                NombreProveedor = itemsProveedor.First().NombreProveedor, 
                FechaCompra = DateTime.Now,
                Items = itemsProveedor.ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermiso("P07_GESTION_COMPRAS")]
        public async Task<IActionResult> Confirmar(ConfirmarCompraViewModel model)
        {
            // 1. Validar y recuperar items de sesión
            var itemsSession = await _cartService.ObtenerItemsPorProveedorAsync(model.IdProveedor);
            
            if (!itemsSession.Any())
            {
                TempData["Error"] = "No hay items en el carrito para este proveedor.";
                return RedirectToAction("Index", "Carrito");
            }

            if (!ModelState.IsValid)
            {
                model.Items = itemsSession.ToList();
                // conservar nombre proveedor para la vista
                model.NombreProveedor = model.NombreProveedor ?? itemsSession.First().NombreProveedor;
                return View("Previsualizar", model);
            }

            // 2. Obtener Usuario Actual
            // Asumo que tu UserService tiene un método para obtener el ID del usuario logueado
            // Si no, puedes obtenerlo del Claim principal si usas Auth estándar.
            var usuario  = _userService.ObtenerUsuarioActual();// Ajusta según tu implementación real
            if (usuario == null)
            {
                TempData["Error"] = "Debe iniciar sesión para confirmar la compra.";
                return RedirectToAction("Login", "Acceso");
            }

            try 
            {
                // 3. Mapeo Manual: ViewModel -> Entidad Compra (Domain)
                var nuevaCompra = new Compra
                {
                    FechaCompra = model.FechaCompra.ToUniversalTime(),
                    Observaciones = model.Observaciones,
                    Estado = src.Models.Common.EstadoCompra.PENDIENTE,
                    Proveedor = new Proveedor { IdProveedor = model.IdProveedor },
                    Usuario = new Usuario { IdUsuario = usuario.IdUsuario }, // Usamos el ID recuperado
                    Detalles = itemsSession.Select(i => new DetalleCompra
                    {
                        Producto = new Producto { IdProducto = i.IdProducto },
                        Cantidad = i.Cantidad,
                        PrecioPactado = i.PrecioUnitario
                    }).ToList()
                };

                // 4. Guardar usando el repositorio de tu compañero
                await _compraRepo.AddAsync(nuevaCompra);

                // 5. Limpiar Carrito
                await _cartService.LimpiarCarritoPorProveedorAsync(model.IdProveedor);

                TempData["Success"] = "Orden de compra generada exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                var mensajeError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                TempData["Error"] = "Error al procesar: " + mensajeError;
                model.Items = itemsSession;
                return View("Previsualizar", model);
            }
        }



        // ==========================================
        // PARTE 2: GESTIÓN DE COMPRAS (NUEVO)
        // ==========================================

        [HttpGet]
        [AuthorizePermiso("P07_GESTION_COMPRAS")]
        public async Task<IActionResult> Index()
        {
            // 1. Traer TODO sin filtros de servidor
            var comprasDom = await _compraRepo.GetAllAsync();
            
            // Ordenamos por fecha descendente para ver lo más nuevo primero
            var listaOrdenada = comprasDom.OrderByDescending(c => c.FechaCompra).ToList();

            // Mapeo manual a VM
            var listaVM = listaOrdenada.Select(c => new ListarCompraViewModel
            {
                IdCompra = c.IdCompra,
                Fecha = c.FechaCompra,
                Estado = c.Estado.ToString(), // "PENDIENTE", "ENVIADA", etc.
                Total = c.TotalOrden,
                ProveedorRazonSocial = c.Proveedor?.RazonSocial ?? "Desc.",
                UsuarioNombre = c.Usuario?.Nombre ?? "-",
                UsuarioApellido = c.Usuario?.Apellido ?? "-",
                Detalles = c.Detalles.Select(d => new ListarDetalleCompraViewModel
                {
                    // Solo mapeo básico necesario para la lista (si lo necesitas)
                    IdDetalleCompra = d.IdDetalleCompra,
                    ProductoNombre = d.Producto?.Nombre ?? "-",
                    Cantidad = d.Cantidad
                }).ToList()
            }).ToList();

            return View(listaVM);
        }
        // Acción para pasar de PENDIENTE a ENVIADA
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermiso("P07_GESTION_COMPRAS")]
        public async Task<IActionResult> MarcarEnviada(int idCompra)
        {
            try
            {
                await _compraRepo.MarcarComoEnviadaAsync(idCompra);
                TempData["Success"] = $"Compra marcada como ENVIADA.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }


        // Acción: CANCELAR Compra (Desde Pendiente o Enviada)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermiso("P07_GESTION_COMPRAS")]
        public async Task<IActionResult> Cancelar(int idCompra, string motivo)
        {
            try
            {
                await _compraRepo.CancelarCompraAsync(idCompra, motivo);
                TempData["Success"] = "Compra cancelada correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cancelar: " + ex.Message;
            }
            // Retornamos al índice respetando el filtro desde donde vino (o default)
            return RedirectToAction(nameof(Index)); 
        }



        // Vista de Detalle para "Gestionar" (Caso ENVIADA)
        [HttpGet]
        [AuthorizePermiso("P07_GESTION_COMPRAS")]
        public async Task<IActionResult> Gestionar(int id)
        {
            var compra = await _compraRepo.GetByIdAsync(id);
            if (compra == null) return NotFound();

            // Reutilizamos el VM de listar o creamos uno de detalle si fuera necesario
           var vm = new GestionarCompraViewModel
            {
                IdCompra = compra.IdCompra,
                Fecha = compra.FechaCompra,
                Estado = compra.Estado.ToString(),
                IdProveedor = compra.Proveedor?.IdProveedor ?? 0,
                ProveedorRazonSocial = compra.Proveedor?.RazonSocial ?? "Desc.",
                UsuarioNombreCompleto = $"{compra.Usuario?.Nombre} {compra.Usuario?.Apellido}",
                Observaciones = compra.Observaciones,
                
                Detalles = compra.Detalles.Select(d => new GestionarDetalleViewModel
                {
                    IdDetalleCompra = d.IdDetalleCompra,
                    IdProducto = d.Producto?.IdProducto ?? 0,
                    ProductoNombre = d.Producto?.Nombre ?? "Desc.",
                    Cantidad = d.Cantidad,
                    PrecioPactado = d.PrecioPactado
                }).ToList()
            };

            return View(vm);
        }


        // Acción: GUARDAR CAMBIOS (Edición de cantidades/precios)
        // Solo válido si está PENDIENTE o ENVIADA
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermiso("P07_GESTION_COMPRAS")]
        public async Task<IActionResult> GuardarCambios(GestionarCompraViewModel model)
        {
            if (!ModelState.IsValid)
            { 
                // TRUCO: Volvemos a cargar la entidad y "parchamos" con los datos del usuario para mostrar el error.
                var compraDb = await _compraRepo.GetByIdAsync(model.IdCompra);
                if(compraDb != null)
                {
                    model.ProveedorRazonSocial = compraDb.Proveedor?.RazonSocial ?? "-";
                    model.Fecha = compraDb.FechaCompra;
                    model.Estado = compraDb.Estado.ToString();
                    model.UsuarioNombreCompleto = $"{compraDb.Usuario?.Nombre} {compraDb.Usuario?.Apellido}";
                    // Mapeamos nombres de productos de nuevo porque el form solo mandó IDs
                    foreach (var detVM in model.Detalles)
                    {
                        // Buscamos el producto original en la compraDb para sacar el nombre
                        var detOriginal = compraDb.Detalles.FirstOrDefault(d => d.IdDetalleCompra == detVM.IdDetalleCompra);
                        if (detOriginal != null)
                        {
                            detVM.ProductoNombre = detOriginal.Producto?.Nombre ?? "Producto";
                        }
                    }
                }
                ModelState.AddModelError(string.Empty, "Por favor corrija los errores en el formulario.");
                return View("Gestionar", model);
            }
            try
            {
                // 1. Recuperar entidad original para validaciones
                var compraOriginal = await _compraRepo.GetByIdAsync(model.IdCompra);
                if (compraOriginal == null) return NotFound();
                
                if (compraOriginal.Estado == EstadoCompra.COMPLETADA || compraOriginal.Estado == EstadoCompra.CANCELADA)
                {
                    TempData["Error"] = "No se puede editar una compra finalizada o cancelada.";
                    return RedirectToAction(nameof(Gestionar), new { id = model.IdCompra });
                }

                var detallesVm = (model.Detalles ?? new List<GestionarDetalleViewModel>())
                            .Where(d => !d.Eliminar)
                            .ToList();

                if (!detallesVm.Any())
                {
                  
                    // Volver a cargar datos auxiliares para la vista
                    var compraDb = await _compraRepo.GetByIdAsync(model.IdCompra);
                    if (compraDb != null)
                    {
                        model.ProveedorRazonSocial = compraDb.Proveedor?.RazonSocial ?? "-";
                        model.Fecha = compraDb.FechaCompra;
                        model.UsuarioNombreCompleto = $"{compraDb.Usuario?.Nombre} {compraDb.Usuario?.Apellido}";
                        model.Estado = compraDb.Estado.ToString();

                         model.Detalles = compraDb.Detalles.Select(d => new GestionarDetalleViewModel
                        {
                            IdDetalleCompra = d.IdDetalleCompra,
                            IdProducto = d.Producto?.IdProducto ?? 0,
                            ProductoNombre = d.Producto?.Nombre ?? "Desc.",
                            Cantidad = d.Cantidad,
                            PrecioPactado = d.PrecioPactado,
                            Eliminar = false
                        }).ToList();
                    }
                    ModelState.AddModelError(string.Empty, "La compra debe tener al menos un producto en el detalle.");
                    return View("Gestionar", model);
                }




                 var idsVm = new HashSet<int>(detallesVm.Select(d => d.IdDetalleCompra));
                 var detallesAEliminar = compraOriginal.Detalles
                                    .Where(d => !idsVm.Contains(d.IdDetalleCompra))
                                    .ToList();

                foreach (var dEl in detallesAEliminar)
                {
                    compraOriginal.Detalles.Remove(dEl);
                }
                 compraOriginal.Observaciones = model.Observaciones;
                // 2. Actualizar Detalle
                // Iteramos sobre los detalles que vienen del formulario
                foreach (var detVM in model.Detalles)
                {
                    var detDom = compraOriginal.Detalles.FirstOrDefault(d => d.IdDetalleCompra == detVM.IdDetalleCompra);
                    if (detDom != null)
                    {
                        detDom.Cantidad = detVM.Cantidad;
                        detDom.PrecioPactado = detVM.PrecioPactado;
                    }else
                        {
                            // Nuevo detalle (si aplica)
                            compraOriginal.Detalles.Add(new src.Models.Domain.DetalleCompra
                            {
                                Producto = new src.Models.Domain.Producto { IdProducto = detVM.IdProducto },
                                Cantidad = detVM.Cantidad,
                                PrecioPactado = Math.Round(detVM.PrecioPactado, 2)
                            });
                        }
                }
                
               

                // 3. Guardar en Repo
                await _compraRepo.UpdateAsync(compraOriginal);

                TempData["Success"] = "Cambios guardados correctamente.";
                return RedirectToAction(nameof(Gestionar), new { id = model.IdCompra });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al guardar: " + ex.Message;
                return RedirectToAction(nameof(Gestionar), new { id = model.IdCompra });
            }
        }
    }
}