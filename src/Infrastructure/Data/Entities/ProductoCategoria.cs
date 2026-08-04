using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace src.Models.CodeFirst;

[Table("producto_categoria")]
public class ProductoCategoria
{
    [Column("id_producto")]
    public short IdProducto { get; set; }
    [ForeignKey(nameof(IdProducto))]
    public Producto Producto { get; set; }

    [Column("id_categoria")]
    public short IdCategoria { get; set; }
    [ForeignKey(nameof(IdCategoria))]
    public Categoria Categoria { get; set; }
}