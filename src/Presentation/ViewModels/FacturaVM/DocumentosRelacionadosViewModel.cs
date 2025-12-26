namespace src.Presentation.ViewModels.FacturaVM;

public class DocumentosRelacionadosViewModel
{
    public int IdFactura { get; set; }
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
    public decimal Total { get; set; }
    public string? Comentario { get; set; }
    public string? Motivo { get; set; }
}

public class ResumenOrdenPagoViewModel
{
    public int IdOrdenPago { get; set; }
    public DateTime? FechaPago { get; set; } // Nullable según tu modelo
    public decimal TotalOrden { get; set; }  // MontoTotal de la OP
    public decimal MontoAplicado { get; set; } // Parte específica aplicada a ESTA factura
    public bool Enviada { get; set; }
    public string Numero { get; set; } = string.Empty;
    public string EstadoDesc => Enviada ? "Enviada" : "Pendiente";
}