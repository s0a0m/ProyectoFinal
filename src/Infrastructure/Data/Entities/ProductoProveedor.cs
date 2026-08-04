using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace src.Models.CodeFirst;

[Table("producto_proveedor")]
public class ProductoProveedor
{
    [Column("id_producto")]
    public short IdProducto { get; set; }

    [ForeignKey("IdProducto")]
    public Producto Producto { get; set; }

    [Column("id_proveedor")]
    public short IdProveedor { get; set; }

    [ForeignKey("IdProveedor")]
    public Proveedor Proveedor { get; set; }

    [Column("precio", TypeName = "numeric(10,2)")]
    public decimal Precio { get; set; }

    [Column("activo")]
    public bool Activo { get; set; } = true;
}

