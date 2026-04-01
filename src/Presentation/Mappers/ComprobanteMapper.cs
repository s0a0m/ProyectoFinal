using src.Core.Contracts;
using src.Presentation.ViewModels.Comprobantes;
using Dom = src.Models.Domain;

namespace src.Presentation.Mappers;

public static class ComprobanteMapper
{
    public static ListarComprobanteViewModel ToListVM(this Dom.Comprobante comprobante)
    {
        return new ListarComprobanteViewModel
        {
            IdComprobante = comprobante.IdComprobante,
            Numero = comprobante.Numero,
            Proveedor = comprobante.Proveedor?.RazonSocial ?? string.Empty,
            Fecha = comprobante.FechaEmision,
            Total = comprobante.Total,
            Tipo = comprobante is Dom.NotaCredito ? "NC" : "ND",
            Motivo = comprobante.Motivo?.Descripcion ?? string.Empty,
            FacturaReferencia = comprobante.FacturaOriginal?.Numero ?? "Sin referencia",
            Comentario = comprobante.Comentario,
        };
    }

    public static IEnumerable<ListarComprobanteViewModel> ToListVM(
        this IEnumerable<Dom.Comprobante> comprobantes
    )
    {
        return comprobantes.Select(c => c.ToListVM());
    }

    public static Dom.Comprobante? ToDomain(this CrearComprobanteViewModel vm)
    {
        Dom.Comprobante? comprobante = vm.TipoComprobante switch
        {
            "NC" => new Dom.NotaCredito(),
            "ND" => new Dom.NotaDebito(),
            _ => null,
        };

        if (comprobante == null)
            return null;

        comprobante.Numero = vm.Numero;
        comprobante.Total = vm.Total;
        comprobante.Comentario = vm.Comentario;
        comprobante.IdFacturaReferencia = vm.IdFactura;
        comprobante.Motivo = new Dom.MotivoComprobante { IdMotivo = vm.IdMotivo };

        return comprobante;
    }

    public static DetalleComprobanteViewModel ToDetalleVM(this Dom.Comprobante comprobante)
    {
        return new DetalleComprobanteViewModel
        {
            IdComprobante = comprobante.IdComprobante,
            Numero = comprobante.Numero,
            Tipo = comprobante is Dom.NotaCredito ? "Nota de Crédito" : "Nota de Débito",
            Total = comprobante.Total,
            FechaEmision = comprobante.FechaEmision,
            Comentario = comprobante.Comentario,
            Motivo = comprobante.Motivo?.Descripcion ?? string.Empty,

            IdProveedor = comprobante.Proveedor?.IdProveedor ?? 0,
            ProveedorRazonSocial = comprobante.Proveedor?.RazonSocial ?? string.Empty,
            ProveedorCuit = comprobante.Proveedor?.Cuit ?? string.Empty,

            IdFactura = comprobante.IdFacturaReferencia ?? 0,
            NumeroFactura = comprobante.FacturaOriginal?.Numero ?? string.Empty,
            FacturaTotalOriginal = comprobante.FacturaOriginal?.TotalFacturado ?? 0,
            FacturaSaldoActual = comprobante.FacturaOriginal?.Saldo ?? 0,
            FacturaPagada = comprobante.FacturaOriginal?.Pagada ?? false,
        };
    }

    public static GestionNotasViewModel ToGestionVM(this GestionNotasData data)
    {
        return new GestionNotasViewModel
        {
            IdProveedor = data.Proveedor.IdProveedor,
            RazonSocial = data.Proveedor.RazonSocial,
            Cuit = data.Proveedor.Cuit,
            SaldoProveedor = data.Proveedor.Saldo,
            MotivosNC = data
                .MotivosNC.Select(m => new MotivoVM
                {
                    IdMotivo = m.IdMotivo,
                    Descripcion = m.Descripcion,
                })
                .ToList(),
            MotivosND = data
                .MotivosND.Select(m => new MotivoVM
                {
                    IdMotivo = m.IdMotivo,
                    Descripcion = m.Descripcion,
                })
                .ToList(),
            Facturas = data
                .Facturas.Select(f => new FacturaConNotasVM
                {
                    IdFactura = f.Factura.IdFactura,
                    NumeroFactura = f.Factura.Numero,
                    FechaEmision = f.Factura.FechaEmision,
                    TotalFacturado = f.Factura.TotalFacturado,
                    Saldo = f.Factura.Saldo,
                    Pagada = f.Factura.Pagada,
                    Comprobantes = f.Comprobantes.Select(c => c.ToListVM()).ToList(),
                })
                .ToList(),
        };
    }
}
