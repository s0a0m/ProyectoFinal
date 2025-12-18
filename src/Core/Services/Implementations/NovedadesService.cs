using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using src.Core.Services.Interfaces;
using src.Repositories.Interfaces;
using src.Presentation.ViewModels.FamiliaVM;
using Dom = src.Models.Domain;
using src.Presentation.ViewModels.NovedadesVM;
using src.Models.CodeFirst;
using src.Contracts;

namespace src.Core.Services.Implementations
{
    public class NovedadesService : INovedadesService
    {
        private readonly INovedadesRepository _novedadesRepo;
        private readonly IProveedorRepository _proveedorRepo;
        private readonly IProductoProveedorRepository _productoProveedorRepository;
        private readonly IProductoCodigoExternoRepository _productoCodigoExternoRepository;
        public NovedadesService(INovedadesRepository novedadesRepo, IProveedorRepository proveedorRepo,IProductoProveedorRepository productoProveedorRepository, IProductoCodigoExternoRepository productoCodigoExternoRepository)
        {
            _novedadesRepo = novedadesRepo;
            _proveedorRepo = proveedorRepo;
            _productoProveedorRepository = productoProveedorRepository;
            _productoCodigoExternoRepository = productoCodigoExternoRepository;
        }

        public async Task CrearNovedadAsync(NovedadesCrearViewModel vm)
        {
            if (vm == null)
                throw new ArgumentNullException(nameof(vm));

            var proveedor = await _proveedorRepo.GetProveedorById(vm.IdProveedor);

            if (proveedor == null)
                throw new Exception("El proveedor especificado no existe.");

            var novedad = new NovedadPendiente
            {
                IdNovedad = 0,
                IdProveedor = vm.IdProveedor,
                CodigoBarraExterno = vm.CodigoBarraExterno,
                NombreSugerido = vm.NombreSugerido,
                PrecioSugerido = vm.PrecioSugerido,
                Estado = Models.Common.EstadoNovedad.PENDIENTE
            };

            await _novedadesRepo.AddAsync(novedad);
        }

        public async Task<IEnumerable<NovedadesListarViewModel>> GetAllNovedadesPendientes()
        {
            var efnovedades = await _novedadesRepo.GetPendientesAsync();
            var novedadesVM = await MapListarNovedadeVM(efnovedades);
            return novedadesVM;
        }

        private async Task<IEnumerable<NovedadesListarViewModel>> MapListarNovedadeVM(IEnumerable<NovedadPendiente> source)
        {
            // 1. Identificar todos los IdProveedor únicos necesarios.
            // Usamos Select() para obtener todos los Ids y Distinct() para obtener solo los únicos.
            var idsProveedores = source
                .Select(x => x.IdProveedor)
                .Distinct()
                .ToList();

            // 2. Ejecutar UNA SOLA consulta masiva (BATCH LOAD)
            // Asumo que tienes un método en tu repositorio de proveedores para esto.
            // **ESTO REEMPLAZA LAS N CONSULTAS.**
            // un for para todos los ids. Porque el metodo solo trae 1 proveedor por vez.

            var proveedores = new List<Dom.Proveedor>();
            foreach (var id in idsProveedores)
            {
                var proveedor = await _proveedorRepo.GetProveedorById(id);
                if (proveedor != null)
                {
                    proveedores.Add(proveedor);
                }
            }
            // 3. Convertir la lista de proveedores a un diccionario para una búsqueda rápida O(1)
            var dictProveedores = proveedores.ToDictionary(p => p.IdProveedor, p => p.RazonSocial);

            var lista = new List<NovedadesListarViewModel>();

            foreach (var x in source)
            {
                // 4. Buscar la Razón Social en el diccionario (MUCHO más rápido que ir a la BD)
                string razonSocial = dictProveedores.GetValueOrDefault(x.IdProveedor, "Proveedor Desconocido");

                lista.Add(new NovedadesListarViewModel
                {
                    IdNovedad = x.IdNovedad,
                    RazonSocialProveedor = razonSocial, // <--- Uso del diccionario O(1)
                    CodigoBarraExterno = x.CodigoBarraExterno,
                    NombreSugerido = x.NombreSugerido,
                    PrecioSugerido = x.PrecioSugerido
                });
            }

            return lista;
        }
        public async Task<int> ObtenerCantidadNovedadesPendientes()
        {
            return await _novedadesRepo.ContarPendientesAsync();
        }

        public async Task AceptarNovedadAsync(ResolverNovedadViewModel model)
        {
            var novedad = await _novedadesRepo.GetByIdAsync(model.IdNovedad);
            if (novedad == null) throw new KeyNotFoundException("La novedad ya no existe.");

            // 1. Verificamos si YA EXISTE la relación
            var relacionExistente = await _productoProveedorRepository
                .GetByProductoAndProveedorAsync(model.IdProductoSeleccionado, novedad.IdProveedor);

            if (relacionExistente != null)
            {
                // CASO 1: Actualizar existente
                relacionExistente.Precio = model.PrecioFinal;
                relacionExistente.StockAsignado = model.StockFinal;
                relacionExistente.Activo = true;
                
                await _productoProveedorRepository.UpdateAsync2(relacionExistente);
            }
            else
            {
                // CASO 2: Crear nueva relación (Usamos la NUEVA sobrecarga sin lista)
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

            // 2. Gestionar el Código de Barras Externo
            bool existeCodigo = await _productoCodigoExternoRepository
                .ExistsAsync(novedad.CodigoBarraExterno);

            if (!existeCodigo)
            {
                // CORRECCIÓN: Usamos el método que recibe primitivos
                await _productoCodigoExternoRepository.AddAsync(
                    model.IdProductoSeleccionado, // short idProducto
                    novedad.IdProveedor,          // short idProveedor
                    novedad.CodigoBarraExterno    // string codigo
                );
            }

            // 3. Finalizar Novedad
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
            // Necesitas un GetById en tu repo de Novedades (si no lo tienes, agrégalo)
            var novedad = await _novedadesRepo.GetByIdAsync(idNovedad); 
            if (novedad == null) throw new KeyNotFoundException("Novedad no encontrada.");

            // Cargar proveedor para mostrar el nombre
            var proveedor = await _proveedorRepo.GetProveedorById(novedad.IdProveedor);

            return new ResolverNovedadViewModel
            {
                IdNovedad = novedad.IdNovedad,
                NombreSugerido = novedad.NombreSugerido,
                CodigoBarraNovedad = novedad.CodigoBarraExterno,
                PrecioSugerido = novedad.PrecioSugerido,
                RazonSocialProveedor = proveedor?.RazonSocial ?? "Desconocido"
            };
        }



    }

}