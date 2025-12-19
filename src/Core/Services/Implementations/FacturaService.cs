using System.Runtime.CompilerServices;
using src.Core.Services.Interfaces;
using src.Interfaces;
using src.Models.Common;
using src.Models.Domain;
using src.Models.Mappers;
using src.Presentation.ViewModels.CompraVM;
using src.Presentation.ViewModels.FacturaVM;
using src.Repositories.Interfaces;

namespace src.Core.Services.Implementations
{
    public class FacturaService : IFacturaService
    {
        private readonly IFacturaRepository _facturaRepository;

        public FacturaService(IFacturaRepository facturaRepo)
        {
            _facturaRepository = facturaRepo;
        }

        public Task<bool> ActualizarEstadoPagoAsync(int id, bool pagada)
        {
            throw new NotImplementedException();
        }

        public Task<int> CrearFacturaAsync(CrearFacturaViewModel modelo)
        {
            throw new NotImplementedException();
        }
        public Task<bool> ExisteNumeroFacturaAsync(short idProveedor, string numero)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ListarFacturaViewModel>> ObtenerPendientesPagoAsync()
        {
            throw new NotImplementedException();
        }
        Task<ListarDetalleFacturaViewModel?> IFacturaService.ObtenerPorIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ListarFacturaViewModel>> ObtenerTodasAsync()
        {
            var facturas = await _facturaRepository.GetAllAsync();

            return facturas.Select(f => new ListarFacturaViewModel
            {
                IdFactura = f.IdFactura,
                NumeroComprobante = f.NumeroFactura,

                Proveedor = f.Proveedor?.RazonSocial ?? "Desconocido",

                Fecha = f.FechaEmision,
                Total = f.TotalFacturado,
                EstadoPago = f.Pagada ? "Pagada" : "Pendiente",
                CantidadItems = f.Detalles?.Count ?? 0
            }).ToList();
        }

    }
}