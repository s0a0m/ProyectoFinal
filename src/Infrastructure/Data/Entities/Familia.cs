using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace src.Models.CodeFirst;

[Table("familia")]
public partial class Familia
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id_familia")]
    public short IdFamilia { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("nombre")]
    public string Nombre { get; set; }

    [Required]
    [MaxLength(200)]
    [Column("descripcion")]
    public string Descripcion { get; set; }

    public ICollection<Categoria> Categorias { get; set; } = new List<Categoria>();
}
