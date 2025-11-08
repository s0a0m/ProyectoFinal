using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace src.Models.CodeFirst;

[Table("producto")]
public class Producto
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id_producto")]
    public short IdProducto { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("nombre")]
    public string Nombre { get; set; }

    [Column("stock_minimo")]
    public int StockMinimo { get; set; }

    [Column("stock_total")]
    public int StockTotal { get; set; }

    [Column("activo")]
    public bool Activo { get; set; }

    [Column("id_grupo")]
    public short IdGrupo { get; set; }
    [ForeignKey("IdGrupo")]
    public Grupo Grupo { get; set; }
    
    [Column("id_categoria")]
    public short IdCategoria { get; set; }
    [ForeignKey("IdCategoria")]
    public Categoria Categoria { get; set; }
    
    [Column("id_codigo_barra")]
    public short IdCodigoBarra { get; set; }
    [ForeignKey("IdCodigoBarra")]
    public CodigoBarra CodigoBarra { get; set; }

    public ICollection<ProductoProveedor> ProductosProveedores { get; set; }
}