using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace src.Models.CodeFirst;

[Table("usuario")]
public partial class usuario
{

    [Key]
    public short id_usuario { get; set; }
    public bool activo { get; set; }

    [Required]
    [StringLength(100)]
    public string correo { get; set; }

    [Required]
    [StringLength(20)]
    public string telefono { get; set; }

    [Required]
    [StringLength(50)]
    public string contrasenia { get; set; }

    [Required]
    [StringLength(50)]
    public string identificacion { get; set; }

    [Required]
    [StringLength(50)]
    public string nombre { get; set; }

    [Required]
    [StringLength(50)]
    public string apellido { get; set; }

    [Column(TypeName = "date")]
    public DateTime fecha_alta { get; set; }
}