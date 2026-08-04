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
    public ICollection<ProductoProveedor> ProductosProveedores { get; set; } = new List<ProductoProveedor>();
    public ICollection<NovedadesProveedor> Novedades { get; set; } = new List<NovedadesProveedor>();
    // public ICollection<ProductoGrupo> ProductosGrupos { get; set; } = new List<ProductoGrupo>();
    public ICollection<ProductoCategoria> ProductosCategorias { get; set; } = new List<ProductoCategoria>();
    public ICollection<ProductoCodigoBarra> ProductoCodigoBarras { get; set; } = new List<ProductoCodigoBarra>();
    public ICollection<ProductoCodigoExterno> CodigosBarrasExternos { get; set; } = new List<ProductoCodigoExterno>();

    public ICollection<UbicacionProducto> UbicacionesProductos { get; set; } = new List<UbicacionProducto>();

    // Y si en el futuro quieres ver el historial de un producto directamente:
    public ICollection<MovimientoStock> Movimientos { get; set; } = new List<MovimientoStock>();
}