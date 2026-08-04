using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace src.Models.CodeFirst;

[Table("detalle_compra")]
public class DetalleCompra
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id_detalle_compra")]
    public int IdDetalleCompra { get; set; }

    [Required]
    [Column("id_compra")]
    public short IdCompra { get; set; }

    [Required]
    [Column("id_producto")]
    public short IdProducto { get; set; }

    [Required]
    [Column("cantidad")]
    public int Cantidad { get; set; }

    [Required]
    [Column("precio_pactado", TypeName = "decimal(18,2)")]
    public decimal PrecioPactado { get; set; }

    // --- Relaciones (Propiedades de Navegación) ---

    [ForeignKey("IdCompra")]
    public virtual Compra Compra { get; set; } = null!;

    [ForeignKey("IdProducto")]
    public virtual Producto Producto { get; set; } = null!;
}