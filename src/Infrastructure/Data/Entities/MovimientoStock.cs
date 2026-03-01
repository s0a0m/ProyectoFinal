using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace src.Models.CodeFirst;

[Table("movimiento_stock")]
public class MovimientoStock
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id_movimiento_stock")]
    public int IdMovimientoStock { get; set; }

    [Column("id_producto")]
    public short IdProducto { get; set; }

    [ForeignKey(nameof(IdProducto))]
    public Producto Producto { get; set; } = new Producto();

    // Origen
    [Column("id_fila_origen")]
    public int? IdFilaOrigen { get; set; } // Puede ser nulo si es un ingreso inicial de proveedor

    [ForeignKey(nameof(IdFilaOrigen))]
    public Fila? FilaOrigen { get; set; }

    // Destino
    [Column("id_fila_destino")]
    public int? IdFilaDestino { get; set; } // Puede ser nulo si es una merma o salida final

    [ForeignKey(nameof(IdFilaDestino))]
    public Fila? FilaDestino { get; set; }

    [Column("cantidad", TypeName = "decimal(18,2)")]
    public decimal Cantidad { get; set; }

    [Column("fecha_movimiento", TypeName = "timestamp without time zone")] // Ideal para PostgreSQL
    public DateTime FechaMovimiento { get; set; }

    [Column("id_usuario")]
    public short IdUsuario { get; set; }

    [ForeignKey(nameof(IdUsuario))]
    public Usuario Usuario { get; set; } = new Usuario();
}