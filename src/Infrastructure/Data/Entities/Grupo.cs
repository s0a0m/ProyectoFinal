using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace src.Models.CodeFirst;

[Table("grupo")]
public class Grupo
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id_grupo")]
    public short IdGrupo { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("nombre")]
    public string Nombre { get; set; }

    [MaxLength(100)]
    [Column("descripcion")]
    public string Descripcion { get; set; }
    public ICollection<ProductoGrupo> ProductosGrupos { get; set; } = new List<ProductoGrupo>();
}
