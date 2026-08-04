namespace src.Presentation.ViewModels.Comprobantes;

public class DetalleComprobanteViewModel
{
    // Datos del comprobante
    public int IdComprobante { get; set; }
    public string Numero { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty; // "Nota de Crédito" o "Nota de Débito"
    public decimal Total { get; set; }
    public DateTime FechaEmision { get; set; }
    public string? Comentario { get; set; }
    public string Motivo { get; set; } = string.Empty;

    // Datos del proveedor
    public int IdProveedor { get; set; }
    public string ProveedorRazonSocial { get; set; } = string.Empty;
    public string ProveedorCuit { get; set; } = string.Empty;

    // Datos de la factura referenciada
    public int IdFactura { get; set; }
    public string NumeroFactura { get; set; } = string.Empty;
    public decimal FacturaTotalOriginal { get; set; }
    public decimal FacturaSaldoActual { get; set; }
    public bool FacturaPagada { get; set; }
}
