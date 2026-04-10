using Microsoft.AspNetCore.Mvc;
using src.Core.Services.Interfaces;
using src.Presentation.ViewModels.CompraVM;
using src.Models.Domain;
using src.Models.Common;
using src.Repositories.Interfaces;
using src.Presentation.Attributes;
using src.Presentation.Mappers;


namespace src.Presentation.Controllers
{
    public class CompraController : Controller
    {
        private readonly ICartService _cartService;
        private readonly ICompraRepository _compraRepo;
        private readonly ICompraService _compraService;
        private readonly IUserService _userService;

        public CompraController(ICartService cartService, ICompraRepository compraRepo, ICompraService compraService, IUserService userService)
        {
            _cartService = cartService;
            _compraRepo = compraRepo;
            _compraService = compraService;
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
                // 3. Mapeo usando el mapper
                var nuevaCompra = model.ToDomain();
                nuevaCompra.FechaCompra = model.FechaCompra.ToUniversalTime();
                nuevaCompra.Estado = EstadoCompra.PENDIENTE;
                nuevaCompra.Usuario = new Usuario { IdUsuario = usuario.IdUsuario };

                // 4. Guardar usando el servicio
                await _compraService.CreateAsync(nuevaCompra);

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
                // Volvemos a cargar la entidad y "parchamos" con los datos del usuario para mostrar el error.
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
                // Validar que haya al menos un detalle
                var detallesVm = (model.Detalles ?? new List<GestionarDetalleViewModel>())
                            .Where(d => !d.Eliminar)
                            .ToList();

                if (!detallesVm.Any())
                {
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

                // Mapear y actualizar usando el servicio
                var compraDom = model.ToDomain();
                await _compraService.UpdateAsync(compraDom);

                TempData["Success"] = "Cambios guardados correctamente.";
                return RedirectToAction(nameof(Gestionar), new { id = model.IdCompra });
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
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
