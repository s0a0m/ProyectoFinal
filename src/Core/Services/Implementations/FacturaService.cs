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
        private readonly ICondicionPagoRepository _condicionPagoRepository;

        public FacturaService(IFacturaRepository facturaRepo, ICondicionPagoRepository condicionPagoRepository)
        {
            _facturaRepository = facturaRepo;
            _condicionPagoRepository = condicionPagoRepository;
        }

        public Task<bool> ActualizarEstadoPagoAsync(int id, bool pagada)
        {
            throw new NotImplementedException();
        }

        public async Task<int> CrearFacturaAsync(CrearFacturaViewModel modelo)
        {
            var nuevaFactura = new Dom.Factura
            {
                Compra = new Compra { IdCompra = modelo.IdCompra },
                Proveedor = new Proveedor { IdProveedor = modelo.IdProveedor },
                NumeroFactura = modelo.NumeroFactura,
                FechaEmision = modelo.FechaEmision,
                CondicionPago = await _condicionPagoRepository.GetByIdAsync(modelo.IdCondicionPago),
                Pagada = false,
                Detalles = new List<Dom.DetalleFactura>()
            };

            decimal acumuladorTotal = 0;

            foreach (var item in modelo.Detalles)
            {
                var detalle = new Dom.DetalleFactura
                {
                    Producto = new Producto { IdProducto = item.IdProducto },
                    Cantidad = item.Cantidad,
                    PrecioUnitario = item.PrecioUnitario
                };

                acumuladorTotal += (item.Cantidad * item.PrecioUnitario);
                nuevaFactura.Detalles.Add(detalle);
            }

            nuevaFactura.TotalFacturado = acumuladorTotal;

            var facturaCreada = await _facturaRepository.AddAsync(nuevaFactura);

            return facturaCreada.IdFactura;
        }
        public async Task<bool> ExisteNumeroFacturaAsync(short idProveedor, string numero)
        {
            return await _facturaRepository.ExisteNumeroFacturaAsync(idProveedor, numero);
        }

        public async Task<IEnumerable<ListarFacturaViewModel>> ObtenerPendientesPagoAsync()
        {
            var facturas = await _facturaRepository.GetPendientesPagoAsync();

            return facturas.Select(f => new ListarFacturaViewModel
            {
                IdFactura = f.IdFactura,
                NumeroComprobante = f.NumeroFactura,
                Proveedor = f.Proveedor?.RazonSocial ?? "Desconocido",
                Fecha = f.FechaEmision,
                Total = f.TotalFacturado,
                EstadoPago = "Pendiente",
                CantidadItems = f.Detalles?.Count ?? 0
            }).ToList();
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

                ProveedorNombre = factura.Proveedor?.RazonSocial ?? "Desconocido",
                CuitProveedor = factura.Proveedor?.Cuit ?? "N/A",

                CondicionPagoDesc = factura.CondicionPago switch
                {
                    Dom.Cuota c => $"{c.Cuotas} cuotas (Interés: {c.InteresPorcentual}%) - Vence a los {c.DiasPago} días",
                    Dom.Contado => $"Contado - Pago a los {factura.CondicionPago.DiasPago} días",
                    _ => factura.CondicionPago != null
                            ? $"Plazo: {factura.CondicionPago.DiasPago} días"
                            : "No especificada"
                },

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

        public async Task<bool> UpdateAsync(int id, ActualizarFacturaViewModel model)
        {
            // throw new NotImplementedException();
            var factura = await _facturaRepository.GetByIdAsync(id);
            if (factura == null) return false;
            if (factura.Proveedor == null)
            {
                throw new InvalidOperationException("No se puede validar el número de factura porque los datos del proveedor no están cargados.");
            }

            if (factura.NumeroFactura != model.NumeroFactura)
            {
                var existe = await _facturaRepository.ExisteNumeroFacturaAsync((short)factura.Proveedor.IdProveedor, model.NumeroFactura);
                if (existe) throw new Exception("El nuevo número de factura ya existe para este proveedor.");
            }

            factura.NumeroFactura = model.NumeroFactura;
            factura.FechaEmision = model.FechaEmision;
            factura.IdCondicionPagoUsada = model.IdCondicionPago;
            factura.Pagada = model.Pagada;

            await _facturaRepository.UpdateAsync(factura);
            return true;
        }
    }
}