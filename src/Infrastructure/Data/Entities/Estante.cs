using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace src.Models.CodeFirst;

[Table("estante")]
public class Estante
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id_estante")]
    public int IdEstante { get; set; }

    [Required]
    [StringLength(50)]
    [Column("numero_estante")]
    public string NumeroEstante { get; set; } = string.Empty;

    [Column("id_deposito")]
    public int IdDeposito { get; set; }

    [ForeignKey(nameof(IdDeposito))]
    public Deposito Deposito { get; set; }

    [Column("activo")]
    public bool Activo { get; set; }

    [Column("tiene_espacio")]
    public bool TieneEspacio { get; set; }

    [StringLength(500)]
    [Column("observaciones")]
    public string? Observaciones { get; set; }

    // Propiedad de navegación inversa
    public ICollection<Fila> Filas { get; set; } = new List<Fila>();
}