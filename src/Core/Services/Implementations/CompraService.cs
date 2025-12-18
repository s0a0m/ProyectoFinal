using System.Runtime.CompilerServices;
using src.Core.Services.Interfaces;
using src.Interfaces;
using src.Models.Common;
using src.Models.Domain;
using src.Models.Mappers;
using src.Presentation.ViewModels.CompraVM;

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

        public async Task<int> CreateAsync(CrearCompraViewModel compraVM, short idUsuario)
        {
            var compra = DominioMapper.Map(compraVM);
            compra.FechaCompra = DateTime.UtcNow;
            compra.Estado = EstadoCompra.PENDIENTE;
            compra.FechaRecepcion = null;
            compra.Usuario.IdUsuario = idUsuario;
            var compraEF = DominioMapper.Map(compra);
            foreach (var detalle in compraEF.Detalles)
            {
                detalle.IdDetalleCompra = 0;
            }
            await _compraRepository.AddAsync(compra);
            return compraEF.IdCompra;
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

        public Task UpdateAsync(Compra compra)
        {
            throw new NotImplementedException();
        }
    }
}