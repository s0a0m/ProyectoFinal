using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using src.Models.Common;

namespace src.Models.CodeFirst;

[Table("detalle_factura")]
public class DetalleFactura
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id_detalle_factura")]
    public int IdDetalleFactura { get; set; }

    [Required]
    [Column("id_factura")]
    public int IdFactura { get; set; }

    [Required]
    [Column("id_producto")]
    public short IdProducto { get; set; }

    [Required]
    [Column("cantidad")]
    public int Cantidad { get; set; }

    [Required]
    [Column("precio_bruto")]
    public decimal PrecioBruto { get; set; }

    [Required]
    [Column("porcentaje_descuento")]
    public decimal PorcentajeDescuento { get; set; } = 0;

    [Required]
    [Column("precio_neto")]
    public decimal PrecioNeto { get; set; }

    // Relaciones
    [ForeignKey("IdFactura")]
    public virtual Factura Factura { get; set; } = null!;

    [ForeignKey("IdProducto")]
    public virtual Producto Producto { get; set; } = null!;
}