using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace src.Models.CodeFirst;

[Table("producto_codigo_externo")]
public class ProductoCodigoExterno
{
    [Column("id_producto")]
    public short IdProducto { get; set; }
    [ForeignKey("IdProducto")]
    public Producto Producto { get; set; }
    [Column("id_proveedor")]
    public short IdProveedor { get; set; }
    [ForeignKey("IdProveedor")]
    public Proveedor Proveedor { get; set; }
    [Required]
    [MaxLength(50)]
    [Column("codigo_barra_proveedor")]
    public string CodigoBarraProveedor { get; set; }
}