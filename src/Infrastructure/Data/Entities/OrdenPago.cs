namespace src.Models.CodeFirst;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("orden_pago")]
public class OrdenPago
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id_orden_pago")]
    public int IdOrdenPago { get; set; }

    [Required]
    [Column("numero_orden")]
    [StringLength(50)]
    public string Numero { get; set; }

    [Required]
    [Column("id_proveedor")]
    public short IdProveedor { get; set; }

    [Required]
    [Column("monto_total")]
    public decimal MontoTotal { get; set; }

    [Column("fecha_pago")]
    public DateTime? FechaPago { get; set; } = null;

    [Required]
    [Column("enviada")]
    public bool Enviada { get; set; } = false;

    [Required]
    [ForeignKey("IdProveedor")]
    public virtual Proveedor Proveedor { get; set; } = null!;

    public virtual ICollection<PagoDetalle> Detalles { get; set; } = new List<PagoDetalle>();
}
