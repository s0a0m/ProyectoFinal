using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using src.Models.Common;

namespace src.Models.CodeFirst;

[Table("compra")]
public class Compra
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id_compra")]
    public short IdCompra { get; set; }

    [Required]
    [Column("id_proveedor")]
    public short IdProveedor { get; set; }

    [Required]
    [Column("id_usuario")]
    public short IdUsuario { get; set; }

    [Required]
    [Column("fecha_compra")]
    public DateTime FechaCompra { get; set; } = DateTime.Now;

    [Column("observaciones")]
    [StringLength(500)]
    public string? Observaciones { get; set; }

    [Required]
    [Column("estado")]
    public EstadoCompra Estado { get; set; } = EstadoCompra.PENDIENTE;

    // Relaciones (Propiedades de Navegación)
    [ForeignKey("IdProveedor")]
    public virtual Proveedor Proveedor { get; set; } = null!;
    [ForeignKey("IdUsuario")]
    public virtual Usuario Usuario { get; set; } = null!;

    public virtual ICollection<DetalleCompra> Detalles { get; set; } = new List<DetalleCompra>();
    public virtual ICollection<Factura> Facturas { get; set; } = new List<Factura>();
}