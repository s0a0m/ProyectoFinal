namespace src.Presentation.ViewModels.Comprobantes;

public class ListarComprobanteViewModel
{
    public int IdComprobante { get; set; }
    public string Numero { get; set; } = string.Empty;
    public string Proveedor { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public decimal Total { get; set; }
    public string Tipo { get; set; } = string.Empty; // "NC" o "ND"
    public string Motivo { get; set; } = string.Empty;
    public string FacturaReferencia { get; set; } = string.Empty;
    public string? Comentario { get; set; }
    public string ColorClase => Tipo == "NC" ? "success" : "danger";
}
