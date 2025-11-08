using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace src.Models.CodeFirst;

[Table("categoria")]
public class Categoria
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id_categoria")]
    public short IdCategoria { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("nombre")]
    public string Nombre { get; set; }

    [MaxLength(100)]
    [Column("descripcion")]
    public string Descripcion { get; set; }

    public ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
