using Dom = src.Models.Domain;

namespace src.Presentation.ViewModels.FacturaVM;

public class ListarFacturaViewModel
{
    public int IdFactura { get; set; }
    public string NumeroComprobante { get; set; } = string.Empty;
    public string Proveedor { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public DateTime FechaPago{get;set;}
    public decimal Total { get; set; }
    public decimal Saldo { get; set; }

    public string EstadoPago { get; set; } = string.Empty;
    public int CantidadItems { get; set; }

    public static List<ListarFacturaViewModel> MapFacturaVM(IEnumerable<Dom.Factura> facturas)
    {
        return facturas.Select(f => new ListarFacturaViewModel
            {
                IdFactura = f.IdFactura,
                NumeroComprobante = f.NumeroFactura,
                Proveedor = f.Proveedor?.RazonSocial ?? "Desconocido",
                Fecha = f.FechaEmision,
                FechaPago = f.FechaPago,
                Total = f.TotalFacturado,
                Saldo = f.Saldo,
                EstadoPago = f.Pagada ? "Pagada" : "Pendiente",
                CantidadItems = f.Detalles?.Count ?? 0
            }).ToList();
    }
    

}