using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using src.Models.Common;

namespace src.Models.CodeFirst;

[Table("novedades_proveedor")]
public class NovedadesProveedor
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id_novedad")]
    public int IdNovedad { get; set; }

    [Column("id_proveedor")]
    public short IdProveedor { get; set; }
    [ForeignKey("IdProveedor")]
    public Proveedor? Proveedor { get; set; }

    [Column("id_producto")]
    public short? IdProducto { get; set; }

    [ForeignKey(nameof(IdProducto))]
    public Producto? Producto { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("codigo_barra_externo")]
    public string CodigoBarraExterno { get; set; }

    [Required]
    [MaxLength(200)]
    [Column("nombre_sugerido")]
    public string NombreSugerido { get; set; }

    [Column("precio_sugerido", TypeName = "numeric(10,2)")]
    public decimal PrecioSugerido { get; set; }

    [Column("stock_sugerido")]
    public int StockSugerido { get; set; }

    [Column("estado")]
    public EstadoNovedad Estado { get; set; } = EstadoNovedad.PENDIENTE;

    [Column("fecha_importacion")]
    public DateTime FechaImportacion { get; set; }
    [Column("observaciones")]
    public string? Observaciones { get; set; }

    [Column("fecha_modificacion")]
    public DateTime? FechaModificacion { get; set; }
}