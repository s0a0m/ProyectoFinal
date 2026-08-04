using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace src.Models.CodeFirst;

[Table("producto_codigo_barra")]
public class ProductoCodigoBarra
{
    [Column("id_producto")]
    public short IdProducto { get; set; }
    [ForeignKey(nameof(IdProducto))]
    public Producto Producto { get; set; }

    [Column("id_codigo_barra")]
    public short IdCodigoBarra { get; set; }
    [ForeignKey(nameof(IdCodigoBarra))]
    public CodigoBarra CodigoBarra { get; set; }
}