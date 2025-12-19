namespace src.Models.Domain;

public class Factura
{
    public int IdFactura { get; set; }
    public string NumeroFactura { get; set; } = string.Empty;
    public DateTime FechaEmision { get; set; }
    public decimal TotalFacturado { get; set; }
    public bool Pagada { get; set; }
    // public short IdCondicionPagoUsada { get; set; }
    public List<DetalleFactura> Detalles { get; set; } = new List<DetalleFactura>();
    public Proveedor? Proveedor { get; set; }
    public Compra? Compra { get; set; }
    public CondicionDePago? CondicionPago { get; set; }
}
