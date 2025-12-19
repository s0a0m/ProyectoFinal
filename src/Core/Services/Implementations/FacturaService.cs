using System.Runtime.CompilerServices;
using src.Core.Services.Interfaces;
using src.Interfaces;
using src.Models.Common;
using src.Models.Domain;
using src.Models.Mappers;
using src.Presentation.ViewModels.CompraVM;
using src.Presentation.ViewModels.FacturaVM;
using src.Repositories.Interfaces;
using Dom = src.Models.Domain;

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
        public async Task<ListarDetalleFacturaViewModel?> ObtenerPorIdAsync(int id)
        {
            var factura = await _facturaRepository.GetByIdAsync(id);

            if (factura == null) return null;

            return new ListarDetalleFacturaViewModel
            {
                IdFactura = factura.IdFactura,
                NumeroComprobante = factura.NumeroFactura,
                Fecha = factura.FechaEmision,
                Total = factura.TotalFacturado,
                EstadoPago = factura.Pagada ? "Pagada" : "Pendiente",

                // Datos del Proveedor (vienen del Include en el Repo)
                ProveedorNombre = factura.Proveedor?.RazonSocial ?? "Desconocido",
                CuitProveedor = factura.Proveedor?.Cuit ?? "N/A",

                // Datos de Condición de Pago
                CondicionPagoDesc = factura.CondicionPago switch
                {
                    Dom.Cuota c => $"{c.Cuotas} cuotas (Interés: {c.InteresPorcentual}%) - Vence a los {c.DiasPago} días",
                    Dom.Contado => $"Contado - Pago a los {factura.CondicionPago.DiasPago} días",
                    _ => factura.CondicionPago != null
                            ? $"Plazo: {factura.CondicionPago.DiasPago} días"
                            : "No especificada"
                },

                // Mapeo de la lista de ítems (Detalles)
                Items = factura.Detalles.Select(d => new ItemFacturaViewModel
                {
                    ProductoNombre = d.Producto?.Nombre ?? "Producto no identificado",
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario,
                    Subtotal = d.Subtotal
                }).ToList()
            };
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