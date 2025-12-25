namespace src.Models.Domain;

public class NotaDebito : Comprobante
{
    // public int IdFacturaReferencia { get; set; }

    public virtual Factura? FacturaOriginal { get; set; }
}