using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace src.Models.CodeFirst;

[Table("fila")]
public class Fila
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id_fila")]
    public int IdFila { get; set; }

    [Required]
    [StringLength(50)]
    [Column("n_fila")]
    public string NFila { get; set; } = string.Empty;

    [Column("id_estante")]
    public int IdEstante { get; set; }

    [ForeignKey(nameof(IdEstante))]
    public Estante Estante { get; set; }

    [Column("activo")]
    public bool Activo { get; set; }

    [Column("tiene_espacio")]
    public bool TieneEspacio { get; set; }

    [StringLength(500)]
    [Column("observaciones")]
    public string? Observaciones { get; set; }

    public ICollection<UbicacionProducto> UbicacionesProductos { get; set; } = new List<UbicacionProducto>();
}