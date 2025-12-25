namespace src.Models.CodeFirst;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("comprobante")]
public abstract class Comprobante
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id_comprobante")]
    public int IdComprobante { get; set; }
    [Required]
    [Column("id_proveedor")]
    public short IdProveedor { get; set; }
    [Required]
    [Column("id_condicion_pago_usada")]
    public short IdCondicionPagoUsada { get; set; }
    [Required]
    [Column("numero_comprobante")]
    [StringLength(50)]
    public string Numero { get; set; } = null!;
    [Required]
    [Column("total")]
    public decimal Total { get; set; }
    [Required]
    [Column("fecha_emision")]
    public DateTime FechaEmision { get; set; }

    [Required]
    [Column("id_motivo")]
    public short IdMotivo { get; set; }

    [Column("comentario")]
    [StringLength(500)]
    public string? Comentario { get; set; }

    [ForeignKey("IdMotivo")]
    public virtual MotivoComprobante Motivo { get; set; } = null!;

    [ForeignKey("IdProveedor")]
    public virtual Proveedor Proveedor { get; set; } = null!;

    [ForeignKey("IdCondicionPagoUsada")]
    public virtual CondicionDePago CondicionPago { get; set; } = null!;
}
