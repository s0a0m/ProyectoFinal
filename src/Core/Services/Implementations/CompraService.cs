using Core.Common;
using src.Core.Services.Interfaces;
using src.Models.Common;
using src.Models.Domain;
using src.Repositories.Interfaces;
using Dom = src.Models.Domain;

namespace src.Core.Services.Implementations
{
    public class CompraService : ICompraService
    {
        private readonly ICompraRepository _compraRepository;
        private readonly ICartService _cartService;

        public CompraService(ICompraRepository compraRepository, ICartService cartService)
        {
            _compraRepository = compraRepository;
            _cartService = cartService;
        }

        public async Task<ServiceResult<int>> ProcesarCompraDesdeCarritoAsync(
            short idProveedor,
            int idUsuario,
            string observaciones
        )
        {
            // 1. Obtener datos frescos de la sesión (Verdad única)
            var cartResult = await _cartService.ObtenerItemsPorProveedorAsync(idProveedor);

            if (!cartResult.Success || !cartResult.Data.Any())
                return ServiceResult<int>.Fail("El carrito está vacío o expiró.");

            try
            {
                // 2. Mapeo de Negocio: DTO a Entidad de Dominio
                var nuevaCompra = new Dom.Compra
                {
                    Proveedor = new Dom.Proveedor { IdProveedor = idProveedor },
                    Usuario = new Dom.Usuario { IdUsuario = (short)idUsuario },
                    Observaciones = observaciones,
                    FechaCompra = DateTime.UtcNow,
                    Estado = EstadoCompra.PENDIENTE,
                    Detalles = cartResult
                        .Data.Select(d => new Dom.DetalleCompra
                        {
                            Producto = new Dom.Producto { IdProducto = d.IdProducto },
                            Cantidad = d.Cantidad,
                            PrecioPactado = d.PrecioUnitario,
                        })
                        .ToList(),
                };

                // 3. Guardar en Base de Datos
                await _compraRepository.AddAsync(nuevaCompra);

                // 4. Limpiar Carrito (Solo si la compra se guardó bien)
                await _cartService.LimpiarCarritoPorProveedorAsync(idProveedor);

                return ServiceResult<int>.Ok(
                    nuevaCompra.IdCompra,
                    "Orden de compra generada exitosamente."
                );
            }
            catch (Exception)
            {
                return ServiceResult<int>.Fail(
                    "Error crítico al procesar la compra en base de datos."
                );
            }
        }

        public async Task<ServiceResult> MarcarEnviadaAsync(int id)
        {
            var compra = await _compraRepository.GetByIdAsync(id);

            if (compra == null)
                return ServiceResult.Fail("La orden no existe.");

            if (compra.Estado != EstadoCompra.PENDIENTE)
                return ServiceResult.Fail(
                    $"No se puede enviar una orden en estado {compra.Estado}."
                );

            await _compraRepository.MarcarComoEnviadaAsync(id);

            return ServiceResult.Ok("Orden enviada exitosamente.");
        }

        public async Task<ServiceResult> CancelarCompraAsync(int id, string motivo)
        {
            var compra = await _compraRepository.GetByIdAsync(id);

            if (compra == null)
                return ServiceResult.Fail("La orden no existe.");

            if (compra.Estado == EstadoCompra.COMPLETADA || compra.Estado == EstadoCompra.CANCELADA)
                return ServiceResult.Fail("No se puede cancelar una orden finalizada.");

            await _compraRepository.CancelarCompraAsync(id, motivo);

            return ServiceResult.Ok("Orden cancelada.");
        }

        public async Task<IEnumerable<Dom.Compra>> GetAllAsync() =>
            await _compraRepository.GetAllAsync();

        public async Task<ServiceResult<Dom.Compra>> GetByIdAsync(int id)
        {
            var compra = await _compraRepository.GetByIdAsync(id);
            return compra == null
                ? ServiceResult<Dom.Compra>.Fail("La orden de compra no existe.")
                : ServiceResult<Dom.Compra>.Ok(compra);
        }

        public async Task<ServiceResult> UpdateAsync(Dom.Compra compraEditada)
        {
            var compraActual = await _compraRepository.GetByIdAsync(compraEditada.IdCompra);
            if (compraActual == null)
                return ServiceResult.Fail("No se encontró la compra.");

            if (compraActual.Estado == EstadoCompra.ENVIADA)
            {
                if (compraActual.Observaciones != compraEditada.Observaciones)
                {
                    compraActual.Observaciones = compraEditada.Observaciones;
                    await _compraRepository.UpdateAsync(compraActual);
                }
                return ServiceResult.Ok("Observaciones actualizadas.");
            }

            if (compraActual.Estado == EstadoCompra.PENDIENTE)
            {
                decimal nuevoTotal = compraEditada.Detalles.Sum(d => d.Cantidad * d.PrecioPactado);
                if (nuevoTotal > BusinessLimits.MAX_TOTAL_COMPRA)
                    return ServiceResult.Fail($"El monto excede el límite permitido.");

                SincronizarDetalles(compraActual, compraEditada);
                compraActual.Observaciones = compraEditada.Observaciones;

                await _compraRepository.UpdateAsync(compraActual);
                return ServiceResult.Ok("Cambios guardados correctamente.");
            }

            return ServiceResult.Fail("La orden no se puede modificar en su estado actual.");
        }

        public ServiceResult ValidarMontoTotal(decimal totalCalculado)
        {
            if (totalCalculado > BusinessLimits.MAX_TOTAL_COMPRA)
            {
                return ServiceResult.Fail(
                    $"El monto total excede el límite permitido de {BusinessLimits.MAX_TOTAL_COMPRA:C2}."
                );
            }
            return ServiceResult.Ok();
        }

        private void SincronizarDetalles(Dom.Compra actual, Dom.Compra editada)
        {
            var idsNuevos = editada.Detalles.Select(d => d.IdDetalleCompra).ToList();
            actual
                .Detalles.Where(d => !idsNuevos.Contains(d.IdDetalleCompra))
                .ToList()
                .ForEach(d => actual.Detalles.Remove(d));

            foreach (var detN in editada.Detalles)
            {
                var existente = actual.Detalles.FirstOrDefault(d =>
                    d.IdDetalleCompra == detN.IdDetalleCompra
                );
                if (existente != null)
                {
                    existente.Cantidad = detN.Cantidad;
                    existente.PrecioPactado = detN.PrecioPactado;
                }
                else
                    actual.Detalles.Add(detN);
            }
        }
    }
}
