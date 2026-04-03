using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore; // Descomentar si usas EF Core 7+ para [PrimaryKey]

namespace src.Models.CodeFirst;

[Table("ubicacion_producto")]
[PrimaryKey(nameof(IdProducto), nameof(IdFila))] 
public class UbicacionProducto 
{
    [Column("id_producto")]
    public short IdProducto { get; set; }
    
    [ForeignKey(nameof(IdProducto))]
    public Producto Producto { get; set; } = new Producto();

    [Column("id_fila")]
    public int IdFila { get; set; }

    [ForeignKey(nameof(IdFila))]
    public Fila Fila { get; set; } = new Fila();

    [Column("cantidad", TypeName = "decimal(18,2)")]
    public decimal Cantidad { get; set; } 
}