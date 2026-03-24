using src.Core.Contracts;
using src.Presentation.ViewModels.CuentaCorrienteVM;
using Dom = src.Models.Domain;

namespace src.Presentation.Mappers;

public static class ProveedorMapper
{
    public static CuentaCorrienteVM ToCuentaCorrienteVM(this CuentaCorrienteData data)
    {
        var vm = new CuentaCorrienteVM
        {
            IdProveedor = data.Proveedor.IdProveedor,
            RazonSocial = data.Proveedor.RazonSocial,
            PersonaResponsable = data.Proveedor.PersonaResponsable,
            Cuit = data.Proveedor.Cuit,
            SaldoTotal = data.Proveedor.Saldo,
            TotalFacturado = data.Facturas.Sum(f => f.TotalFacturado),
            TotalNC = data.Comprobantes.Where(c => c is Dom.NotaCredito).Sum(c => c.Total),
            TotalND = data.Comprobantes.Where(c => c is Dom.NotaDebito).Sum(c => c.Total),
        };

        // Calcular total pagado desde ordenes confirmadas
        vm.TotalPagado = data.OrdenesPago.Sum(o => o.MontoTotal);

        vm.MotivosNC = data
            .MotivosNC.Select(m => new MotivoCCVM
            {
                IdMotivo = m.IdMotivo,
                Descripcion = m.Descripcion,
            })
            .ToList();

        vm.MotivosND = data
            .MotivosND.Select(m => new MotivoCCVM
            {
                IdMotivo = m.IdMotivo,
                Descripcion = m.Descripcion,
            })
            .ToList();

        foreach (var factura in data.Facturas.OrderByDescending(f => f.FechaEmision))
        {
            // Comprobantes de esta factura
            var notasFactura = data
                .Comprobantes.Where(c => c.IdFacturaReferencia == factura.IdFactura)
                .ToList();

            // Pagos de esta factura
            var pagosFactura = data
                .OrdenesPago.Where(o =>
                    o.Detalles != null && o.Detalles.Any(d => d.IdFactura == factura.IdFactura)
                )
                .ToList();

            var facturaVM = new FacturaCCVM
            {
                IdFactura = factura.IdFactura,
                NumeroFactura = factura.Numero,
                FechaEmision = factura.FechaEmision,
                TotalFacturado = factura.TotalFacturado,
                Saldo = factura.Saldo,
                Pagada = factura.Pagada,
                PuedeEditarse = factura.PuedeEditarse && !notasFactura.Any() && !pagosFactura.Any(),
                Notas = notasFactura
                    .Select(c => new NotaCCVM
                    {
                        IdComprobante = c.IdComprobante,
                        Tipo = c is Dom.NotaCredito ? "NC" : "ND",
                        Numero = c.Numero,
                        Motivo = c.Motivo?.Descripcion ?? string.Empty,
                        Total = c.Total,
                        FechaEmision = c.FechaEmision,
                    })
                    .ToList(),
                Pagos = pagosFactura
                    .Select(o => new PagoCCVM
                    {
                        IdOrdenPago = o.IdOrdenPago,
                        Numero = o.Numero,
                        FechaPago = o.FechaPago,
                        Enviada = o.Enviada,
                        MontoAplicado =
                            o.Detalles?.Where(d => d.IdFactura == factura.IdFactura)
                                .Sum(d => d.MontoAplicado)
                            ?? 0,
                    })
                    .ToList(),
            };

            vm.Facturas.Add(facturaVM);
        }

        return vm;
    }
}
