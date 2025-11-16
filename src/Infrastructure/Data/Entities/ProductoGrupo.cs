using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace src.Models.CodeFirst;

[Table("producto_grupo")]
public class ProductoGrupo
{
    [Column("id_producto")]
    public short IdProducto { get; set; }
    [ForeignKey(nameof(IdProducto))]
    public Producto Producto { get; set; }
    [Column("id_grupo")]
    public short IdGrupo { get; set; }
    [ForeignKey(nameof(IdGrupo))]
    public Grupo Grupo { get; set; }
}