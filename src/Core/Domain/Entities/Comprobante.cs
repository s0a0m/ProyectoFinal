namespace src.Models.Domain;

public abstract class Comprobante
{
    public int IdComprobante { get; set; }
    public string Numero { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public DateTime FechaEmision { get; set; }

    public string? Comentario { get; set; }
    public int? IdFacturaReferencia { get; set; }

    // Navegación (Opcional en dominio puro, pero útil si usas el mismo modelo)
    public Proveedor? Proveedor { get; set; }
    public MotivoComprobante? Motivo { get; set; }
    public CondicionDePago? CondicionPago { get; set; }
    public virtual Factura? FacturaOriginal { get; set; }

}