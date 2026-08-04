namespace src.Models.Domain;

public class PagoDetalle
{
    // public int IdPagoDetalle { get; set; }
    public int IdFactura { get; set; }
    public decimal MontoAplicado { get; set; }
    public virtual Factura? Factura { get; set; }
}