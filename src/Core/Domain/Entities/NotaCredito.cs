namespace src.Models.Domain;

public class NotaCredito : Comprobante
{
    // public int IdFacturaReferencia { get; set; }

    public virtual Factura? FacturaOriginal { get; set; }
}