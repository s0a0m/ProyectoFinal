using Dom = src.Models.Domain;

namespace src.Presentation.ViewModels.FacturaVM;

public class ListarDetalleFacturaViewModel
{
    public int IdFactura { get; set; }
    public string NumeroComprobante { get; set; } = null!;
    public int? IdCompra { get; set; }
    public DateTime Fecha { get; set; }
    public DateTime FechaPago { get; set; }
    public decimal Total { get; set; }
    public decimal Saldo { get; set; }
    public string EstadoPago { get; set; } = string.Empty;
    public string ProveedorNombre { get; set; } = string.Empty;
    public string CuitProveedor { get; set; } = string.Empty;
    public string CondicionPagoDesc { get; set; } = string.Empty;

    public List<ItemFacturaViewModel> Items { get; set; } = new();

    public static ListarDetalleFacturaViewModel MapListarDetalleVM(Dom.Factura factura)
    {
        return new ListarDetalleFacturaViewModel
        {
            IdFactura = factura.IdFactura,
            NumeroComprobante = factura.Numero,
            Fecha = factura.FechaEmision,
            FechaPago = factura.FechaPago,
            Total = factura.TotalFacturado,
            Saldo = factura.Saldo,
            EstadoPago = factura.Pagada ? "Pagada" : "Pendiente",

            ProveedorNombre = factura.Proveedor?.RazonSocial ?? "Desconocido",
            CuitProveedor = factura.Proveedor?.Cuit ?? "N/A",

            CondicionPagoDesc = factura.CondicionPago switch
            {
                Dom.Cuota c =>
                    $"{c.Cuotas} cuotas (Interés: {c.InteresPorcentual}%) - Vence a los {c.DiasPago} días",
                Dom.Contado => $"Contado - Pago a los {factura.CondicionPago.DiasPago} días",
                _ => factura.CondicionPago != null
                    ? $"Plazo: {factura.CondicionPago.DiasPago} días"
                    : "No especificada",
            },

            Items = factura
                .Detalles.Select(d => new ItemFacturaViewModel
                {
                    ProductoNombre = d.Producto?.Nombre ?? "Producto no identificado",
                    Cantidad = d.Cantidad,
                    PrecioBruto = d.PrecioBruto,
                    PorcentajeDescuento = d.PorcentajeDescuento,
                    PrecioNeto = d.PrecioNeto,
                    Subtotal = d.Subtotal,
                })
                .ToList(),
        };
    }
}

public class ItemFacturaViewModel
{
    public string ProductoNombre { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal PrecioBruto { get; set; }
    public decimal PrecioNeto { get; set; }
    public decimal Subtotal { get; set; }
    public decimal PorcentajeDescuento { get; set; } = 0;
}
