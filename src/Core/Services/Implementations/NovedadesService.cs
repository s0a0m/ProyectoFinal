using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq; // Necesario para Select, Distinct, ToList
using src.Core.Services.Interfaces;
using src.Repositories.Interfaces;
using src.Presentation.ViewModels.FamiliaVM;
using Dom = src.Models.Domain;
using src.Presentation.ViewModels.NovedadesVM;
using src.Models.CodeFirst; // Asumo que aquí está NovedadesProveedor / NovedadPendiente
using src.Contracts;

namespace src.Core.Services.Implementations
{
    public class NovedadesService : INovedadesService
    {
        private readonly INovedadesRepository _novedadesRepo;
        private readonly IProveedorRepository _proveedorRepo;
        private readonly IProductoProveedorRepository _productoProveedorRepository;
        private readonly IProductoCodigoExternoRepository _productoCodigoExternoRepository;

        // 1. NUEVA DEPENDENCIA: Necesitamos buscar nombres de productos
        private readonly IProductoRepository _productoRepo;

        public NovedadesService(
            INovedadesRepository novedadesRepo,
            IProveedorRepository proveedorRepo,
            IProductoProveedorRepository productoProveedorRepository,
            IProductoCodigoExternoRepository productoCodigoExternoRepository,
            IProductoRepository productoRepo) // <--- Inyectar aquí
        {
            _novedadesRepo = novedadesRepo;
            _proveedorRepo = proveedorRepo;
            _productoProveedorRepository = productoProveedorRepository;
            _productoCodigoExternoRepository = productoCodigoExternoRepository;
            _productoRepo = productoRepo;
        }

        public async Task CrearNovedadAsync(NovedadesCrearViewModel vm)
        {
            if (vm == null) throw new ArgumentNullException(nameof(vm));

            var proveedor = await _proveedorRepo.GetProveedorById(vm.IdProveedor);
            if (proveedor == null) throw new Exception("El proveedor especificado no existe.");

            var novedad = new NovedadPendiente
            {
                IdNovedad = 0, // Se generará en la DB
                IdProveedor = vm.IdProveedor,
                CodigoBarraExterno = vm.CodigoBarraExterno,
                NombreSugerido = vm.NombreSugerido,
                PrecioSugerido = vm.PrecioSugerido,
                StockSugerido = 0, // Asignamos 0 o null si tu VM no tiene stock
                Estado = Models.Common.EstadoNovedad.PENDIENTE,
                FechaImportacion = DateTime.UtcNow,
                Observaciones = "Carga Manual" // Opcional: para diferenciar de la carga masiva
            };

            await _novedadesRepo.AddAsync(novedad);
        }

        public async Task<IEnumerable<NovedadesListarViewModel>> GetAllNovedadesPendientes()
        {
            var efnovedades = await _novedadesRepo.GetPendientesAsync();
            var novedadesVM = await MapListarNovedadeVM(efnovedades);
            return novedadesVM;
        }

        // --- MÉTODO PRINCIPAL EDITADO ---
        private async Task<IEnumerable<NovedadesListarViewModel>> MapListarNovedadeVM(IEnumerable<NovedadPendiente> source)
        {
            // ---------------------------------------------------------
            // PASO 1: Obtener Proveedores (Tu lógica original)
            // ---------------------------------------------------------
            var idsProveedores = source.Select(x => x.IdProveedor).Distinct().ToList();
            var proveedores = new List<Dom.Proveedor>();

            foreach (var id in idsProveedores)
            {
                var prov = await _proveedorRepo.GetProveedorById(id);
                if (prov != null) proveedores.Add(prov);
            }
            var dictProveedores = proveedores.ToDictionary(p => p.IdProveedor, p => p.RazonSocial);

            // ---------------------------------------------------------
            // PASO 2: Obtener Productos Existentes (Nombres y Precios actuales)
            // ---------------------------------------------------------
            // Filtramos las novedades que YA tienen un ID de producto asignado (Son actualizaciones)
            var novedadesConProducto = source.Where(x => x.IdProducto > 0).ToList();

            // Diccionarios para acceso rápido
            var dictNombresProductos = new Dictionary<short, string>();
            var dictPreciosActuales = new Dictionary<(short IdProd, short IdProv), decimal>();

            if (novedadesConProducto.Any())
            {
                // A. Buscar Nombres de Productos
                var idsProductos = novedadesConProducto.Select(x => x.IdProducto).Distinct().ToList();
                foreach (var idProd in idsProductos)
                {
                    // Idealmente usar un método GetByIds(list) en el repo, pero mantenemos el loop por ahora
                    var prod = await _productoRepo.GetByIdAsync(idProd);
                    if (prod != null)
                    {
                        dictNombresProductos[idProd] = prod.Nombre;
                    }
                }

                // B. Buscar Precios Actuales en ProductoProveedor
                // Esto es para saber cuánto costaba antes de esta novedad
                foreach (var item in novedadesConProducto)
                {
                    var relacion = await _productoProveedorRepository.GetByProductoAndProveedorAsync(item.IdProducto, item.IdProveedor);
                    if (relacion != null)
                    {
                        dictPreciosActuales[(item.IdProducto, item.IdProveedor)] = relacion.Precio;
                    }
                }
            }

            // ---------------------------------------------------------
            // PASO 3: Construir el ViewModel final
            // ---------------------------------------------------------
            var lista = new List<NovedadesListarViewModel>();

            foreach (var x in source)
            {
                string razonSocial = dictProveedores.GetValueOrDefault(x.IdProveedor, "Proveedor Desconocido");

                // Buscar datos adicionales si es una actualización
                string? nombreActual = null;
                decimal? precioActual = null;

                if (x.IdProducto > 0)
                {
                    nombreActual = dictNombresProductos.GetValueOrDefault(x.IdProducto);

                    // Usamos una tupla (IdProducto, IdProveedor) como clave compuesta
                    if (dictPreciosActuales.TryGetValue((x.IdProducto, x.IdProveedor), out decimal precioEncontrado))
                    {
                        precioActual = precioEncontrado;
                    }
                }

                lista.Add(new NovedadesListarViewModel
                {
                    IdNovedad = x.IdNovedad,

                    // Datos del Excel / Novedad
                    RazonSocialProveedor = razonSocial,
                    CodigoBarraExterno = x.CodigoBarraExterno,
                    NombreSugerido = x.NombreSugerido,
                    PrecioSugerido = x.PrecioSugerido,
                    StockSugerido = x.StockSugerido,

                    // Datos de Auditoría
                    Estado = x.Estado.ToString(),
                    FechaImportacion = x.FechaImportacion,
                    Observaciones = x.Observaciones,

                    // Contexto del Sistema (Datos para comparar)
                    IdProductoExistente = x.IdProducto > 0 ? (int)x.IdProducto : null,
                    NombreProductoActual = nombreActual,
                    PrecioActualSistema = precioActual
                });
            }

            return lista;
        }

        public async Task<int> ObtenerCantidadNovedadesPendientes()
        {
            return await _novedadesRepo.ContarPendientesAsync();
        }

        // ... El resto de métodos (AceptarNovedadAsync, Rechazar, ObtenerDatos) quedan igual ...

        public async Task AceptarNovedadAsync(ResolverNovedadViewModel model)
        {
            // (Tu implementación existente está bien, no requiere cambios por el VM de listado)
            // ... copia tu código anterior aquí ...
            var novedad = await _novedadesRepo.GetByIdAsync(model.IdNovedad);
            if (novedad == null) throw new KeyNotFoundException("La novedad ya no existe.");

            var relacionExistente = await _productoProveedorRepository
                .GetByProductoAndProveedorAsync(model.IdProductoSeleccionado, novedad.IdProveedor);

            if (relacionExistente != null)
            {
                relacionExistente.Precio = model.PrecioFinal;
                relacionExistente.StockAsignado = model.StockFinal;
                relacionExistente.Activo = true;
                await _productoProveedorRepository.UpdateAsync2(relacionExistente);
            }
            else
            {
                var nuevaRelacion = new Dom.ProductoProveedor
                {
                    Producto = new Dom.Producto { IdProducto = model.IdProductoSeleccionado },
                    Proveedor = new Dom.Proveedor { IdProveedor = novedad.IdProveedor },
                    Precio = model.PrecioFinal,
                    StockAsignado = model.StockFinal,
                    Activo = true
                };
                await _productoProveedorRepository.AddAsync(nuevaRelacion);
            }

            bool existeCodigo = await _productoCodigoExternoRepository
                .ExistsAsync(novedad.CodigoBarraExterno, novedad.IdProveedor);

            if (!existeCodigo)
            {
                await _productoCodigoExternoRepository.AddAsync(
                    model.IdProductoSeleccionado,
                    novedad.IdProveedor,
                    novedad.CodigoBarraExterno
                );
            }

            novedad.Estado = Models.Common.EstadoNovedad.ACEPTADO;
            novedad.IdProducto = model.IdProductoSeleccionado;
            await _novedadesRepo.UpdateAsync(novedad);
        }

        public async Task RechazarNovedadAsync(int idNovedad)
        {
            var novedad = await _novedadesRepo.GetByIdAsync(idNovedad);
            if (novedad != null)
            {
                novedad.Estado = Models.Common.EstadoNovedad.RECHAZADO;
                await _novedadesRepo.UpdateAsync(novedad);
            }
        }

        public async Task<ResolverNovedadViewModel> ObtenerDatosParaResolverAsync(int idNovedad)
        {
            var novedad = await _novedadesRepo.GetByIdAsync(idNovedad);
            if (novedad == null) throw new KeyNotFoundException("Novedad no encontrada.");

            var proveedor = await _proveedorRepo.GetProveedorById(novedad.IdProveedor);

            return new ResolverNovedadViewModel
            {
                IdNovedad = novedad.IdNovedad,
                IdProveedor = novedad.IdProveedor, // <--- Nuevo

                // Datos del Excel
                NombreSugerido = novedad.NombreSugerido,
                CodigoBarraNovedad = novedad.CodigoBarraExterno,
                PrecioSugerido = novedad.PrecioSugerido,
                StockSugerido = novedad.StockSugerido, // <--- ¡IMPORTANTE! Mapear esto

                RazonSocialProveedor = proveedor?.RazonSocial ?? "Desconocido",

                // Valores por defecto para los inputs (Sugiere lo que vino del Excel)
                PrecioFinal = novedad.PrecioSugerido,
                StockFinal = novedad.StockSugerido
            };
        }
    }
}