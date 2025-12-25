namespace src.Models.CodeFirst;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("nota_debito")]
public class NotaDebito : Comprobante
{
    [Column("id_factura_referencia")]
    public int IdFacturaReferencia { get; set; }

    [ForeignKey("IdFacturaReferencia")]
    public virtual Factura FacturaOriginal { get; set; } = null!;
}
