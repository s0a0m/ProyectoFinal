namespace src.Models.CodeFirst;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("motivo_comprobante")]
public class MotivoComprobante
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id_motivo_comprobante")]
    public short IdMotivoComprobante { get; set; }

    [Required]
    [Column("descripcion")]
    [StringLength(100)]
    public string Descripcion { get; set; } = null!;

    [Required]
    [Column("clase_comprobante")]
    [StringLength(10)]
    public string ClaseComprobante { get; set; } = null!;
}