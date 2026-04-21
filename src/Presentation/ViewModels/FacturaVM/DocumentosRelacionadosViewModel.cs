using System.ComponentModel.DataAnnotations;

namespace src.Presentation.ViewModels.FacturaVM;

public class DocumentosRelacionadosViewModel
{
    public int IdFactura { get; set; }
    public short IdProveedor { get; set; }
    public decimal SaldoFactura { get; set; }
    public string NumeroFactura { get; set; } = string.Empty;
    public string Proveedor { get; set; } = string.Empty;

    public List<ResumenComprobanteViewModel> Comprobantes { get; set; } = new();
    public List<ResumenOrdenPagoViewModel> OrdenesPago { get; set; } = new();
}

public class ResumenComprobanteViewModel
{
    public int IdComprobante { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public DateTime FechaEmision { get; set; }

    [Range(0, 9999999999.99)]
    public decimal Total { get; set; }

    public string? Comentario { get; set; }
    public string? Motivo { get; set; }
}

public class ResumenOrdenPagoViewModel
{
    public int IdOrdenPago { get; set; }
    public DateTime? FechaPago { get; set; } // Nullable según tu modelo

    [Range(0, 9999999999.99)]
    public decimal TotalOrden { get; set; } // MontoTotal de la OP

    [Range(0, 9999999999.99)]
    public decimal MontoAplicado { get; set; } // Parte específica aplicada a ESTA factura

    public bool Enviada { get; set; }
    public string Numero { get; set; } = string.Empty;
    public string EstadoDesc => Enviada ? "Enviada" : "Pendiente";
}
