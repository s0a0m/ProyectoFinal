namespace src.Models.CodeFirst;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("nota_credito")]
public class NotaCredito : Comprobante
{
    [Column("id_factura_referencia")]
    public int IdFacturaReferencia { get; set; }

    [ForeignKey("IdFacturaReferencia")]
    public virtual Factura FacturaOriginal { get; set; } = null!;
}