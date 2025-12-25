namespace src.Models.CodeFirst;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("pago_detalle")]
public class PagoDetalle
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id_pago_detalle")]
    public int IdPagoDetalle { get; set; }

    [Column("id_orden_pago")]
    public int IdOrdenPago { get; set; }

    [Column("id_factura")]
    public int IdFactura { get; set; }

    [Column("monto_aplicado")]
    public decimal MontoAplicado { get; set; }

    [ForeignKey("IdOrdenPago")]
    public virtual OrdenPago OrdenPago { get; set; } = null!;

    [ForeignKey("IdFactura")]
    public virtual Factura Factura { get; set; } = null!;
}