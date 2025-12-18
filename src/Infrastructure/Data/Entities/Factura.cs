using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using src.Models.Common;

namespace src.Models.CodeFirst;

[Table("factura")]
public class Factura
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id_factura")]
    public int IdFactura { get; set; }

    [Required]
    [Column("id_compra")]
    public short IdCompra { get; set; }

    [Required]
    [Column("id_proveedor")]
    public short IdProveedor { get; set; }

    [Required]
    [Column("numero_factura")]
    [StringLength(50)]
    public string NumeroFactura { get; set; } = null!;

    [Required]
    [Column("fecha_emision")]
    public DateTime FechaEmision { get; set; }

    [Required]
    [Column("total_facturado")]
    public decimal TotalFacturado { get; set; }

    [Required]
    [Column("pagada")]
    public bool Pagada { get; set; } = false;

    // Relaciones
    [ForeignKey("IdCompra")]
    public virtual Compra Compra { get; set; } = null!;

    [ForeignKey("IdProveedor")]
    public virtual Proveedor Proveedor { get; set; } = null!;

    public virtual ICollection<DetalleFactura> Detalles { get; set; } = new List<DetalleFactura>();
}