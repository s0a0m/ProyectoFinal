using src.Presentation.ViewModels.CuentaCorrienteVM;
using Dom = src.Models.Domain;

namespace src.Presentation.Mappers;

public static class OrdenPagoMapper
{
    // Domain → VM (listado)
    public static OrdenPagoIndexVM ToIndexVM(this Dom.OrdenPago orden)
    {
        return new OrdenPagoIndexVM
        {
            IdOrdenPago = orden.IdOrdenPago,
            Numero = orden.Numero,
            FechaPago = orden.FechaPago ?? DateTime.MinValue,
            MontoTotal = orden.MontoTotal,
            Enviada = orden.Enviada,
            CantidadFacturasAfectadas = orden.Detalles?.Count ?? 0,
        };
    }

    public static IEnumerable<OrdenPagoIndexVM> ToIndexVM(this IEnumerable<Dom.OrdenPago> ordenes)
    {
        return ordenes.Select(o => o.ToIndexVM());
    }

    // Domain → VM (detalle modal)
    public static OrdenPagoDetalleModalVM ToDetalleModalVM(this Dom.OrdenPago orden)
    {
        return new OrdenPagoDetalleModalVM
        {
            IdOrdenPago = orden.IdOrdenPago,
            NumeroOrden = orden.Numero,
            FechaPago = orden.FechaPago ?? DateTime.MinValue,
            MontoTotal = orden.MontoTotal,
            Estado = orden.Enviada ? "Enviada" : "Pendiente",
            Items =
                orden
                    .Detalles?.Select(d => new DetallePagoItemVM
                    {
                        NumeroFactura = d.Factura?.Numero ?? "S/N",
                        FechaEmisionFactura = d.Factura?.FechaEmision ?? DateTime.MinValue,
                        ImportePagado = d.MontoAplicado,
                    })
                    .ToList()
                ?? new(),
        };
    }

    // Facturas pendientes → VM (formulario de creación)
    public static OrdenPagoFormVM ToFormVM(
        this IEnumerable<Dom.Factura> facturasPendientes,
        short idProveedor,
        string razonSocial
    )
    {
        return new OrdenPagoFormVM
        {
            IdProveedor = idProveedor,
            RazonSocialProveedor = razonSocial,
            FechaPago = DateTime.Now,
            FacturasDisponibles = facturasPendientes
                .Select(f => new SeleccionFacturaVM
                {
                    IdFactura = f.IdFactura,
                    NumeroFactura = f.Numero,
                    FechaEmision = f.FechaEmision,
                    TotalFacturado = f.TotalFacturado,
                    SaldoPendiente = f.Saldo,
                    MontoAPagar = f.Saldo,
                    EstaSeleccionada = false,
                })
                .ToList(),
        };
    }

    // VM → Domain (crear orden)
    public static Dom.OrdenPago ToDomain(this OrdenPagoFormVM vm)
    {
        var orden = new Dom.OrdenPago
        {
            IdProveedor = (short)vm.IdProveedor,
            Numero = vm.NumeroOrden,
            Detalles = new List<Dom.PagoDetalle>(),
        };

        decimal sumaTotal = 0;

        foreach (
            var item in vm.FacturasDisponibles.Where(x => x.EstaSeleccionada && x.MontoAPagar > 0)
        )
        {
            orden.Detalles.Add(
                new Dom.PagoDetalle { IdFactura = item.IdFactura, MontoAplicado = item.MontoAPagar }
            );
            sumaTotal += item.MontoAPagar;
        }

        orden.MontoTotal = sumaTotal;
        return orden;
    }
}
