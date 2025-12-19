namespace src.Presentation.ViewModels.CompraVM;

public class ListarFacturaViewModel
{
    public int IdFactura { get; set; }
    public string NumeroComprobante { get; set; } = string.Empty;
    public string Proveedor { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public decimal Total { get; set; }
    public string EstadoPago { get; set; } = string.Empty;
    public int CantidadItems { get; set; }
}