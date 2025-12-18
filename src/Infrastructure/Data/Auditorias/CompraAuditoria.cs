using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace src.Models.CodeFirst.Auditoria;

[Table("auditoria_compras")]
public class CompraAuditoria
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int IdCompra { get; set; }

    [MaxLength(100)]
    public string Usuario { get; set; } = string.Empty;

    public DateTime Fecha { get; set; } = DateTime.UtcNow;

    [MaxLength(50)]
    public string Accion { get; set; } = string.Empty;

    [Column(TypeName = "text")]
    public string Detalles { get; set; } = string.Empty;

    [Column(TypeName = "text")]
    public string Motivo { get; set; } = string.Empty;
}