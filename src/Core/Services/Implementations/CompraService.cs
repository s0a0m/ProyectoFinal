using System.Runtime.CompilerServices;
using src.Core.Services.Interfaces;
using src.Models.Common;
using src.Models.Domain;
using src.Models.Mappers;
using src.Presentation.ViewModels.CompraVM;
using src.Repositories.Interfaces;
using Dom = src.Models.Domain;

namespace src.Core.Services.Implementations
{
    public class CompraService : ICompraService
    {
        private readonly ICompraRepository _compraRepository;

        public CompraService(ICompraRepository compraRepository)
        {
            _compraRepository = compraRepository;
        }

        public Task CancelarCompraAsync(short id)
        {
            throw new NotImplementedException();
        }

        public async Task<int> CreateAsync(Dom.Compra compra)
        {
            await _compraRepository.AddAsync(compra);
            return compra.IdCompra;
        }

        public Task EsEditableAsync(short id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ListarCompraViewModel>> GetAllAsync()
        {
            var compras = await _compraRepository.GetAllAsync();
            return DominioMapper.MapToRead(compras);
        }

        public async Task<ListarCompraViewModel?> GetByIdAsync(short id)
        {
            var compra = await _compraRepository.GetByIdAsync(id);
            return compra == null ? null : DominioMapper.MapToRead(compra);
        }

        public async Task UpdateAsync(Compra compra)
        {
            // Obtener la compra actual del repositorio
            var compraActual = await _compraRepository.GetByIdAsync(compra.IdCompra);
            if (compraActual == null)
            {
                throw new InvalidOperationException($"No se encontró la compra con ID {compra.IdCompra}");
            }

            // Validar que el estado NO sea COMPLETADA ni CANCELADA
            if (compraActual.Estado == EstadoCompra.COMPLETADA || compraActual.Estado == EstadoCompra.CANCELADA)
            {
                throw new InvalidOperationException("No se puede editar una compra finalizada o cancelada.");
            }

            // Actualizar observaciones
            compraActual.Observaciones = compra.Observaciones;

            // Sincronizar la lista de detalles
            var idsNuevos = new HashSet<int>(compra.Detalles.Select(d => d.IdDetalleCompra));
            
            // Remover detalles que ya no están en la nueva lista
            var detallesAEliminar = compraActual.Detalles
                .Where(d => !idsNuevos.Contains(d.IdDetalleCompra))
                .ToList();
            
            foreach (var detalle in detallesAEliminar)
            {
                compraActual.Detalles.Remove(detalle);
            }

            // Actualizar o agregar detalles
            foreach (var detalleNuevo in compra.Detalles)
            {
                var detalleExistente = compraActual.Detalles
                    .FirstOrDefault(d => d.IdDetalleCompra == detalleNuevo.IdDetalleCompra);
                
                if (detalleExistente != null)
                {
                    // Actualizar detalle existente
                    detalleExistente.Cantidad = detalleNuevo.Cantidad;
                    detalleExistente.PrecioPactado = detalleNuevo.PrecioPactado;
                }
                else
                {
                    // Agregar nuevo detalle
                    compraActual.Detalles.Add(detalleNuevo);
                }
            }

            // Llamar al repositorio para persistir los cambios
            await _compraRepository.UpdateAsync(compraActual);
        }

        public async Task<Dom.Compra?> ObtenerPorIdAsync(int id)
        {
            return await _compraRepository.GetByIdAsync(id);
        }
    }
}

