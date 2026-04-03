using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace src.Models.CodeFirst;

[Table("usuario")]
public partial class Usuario
{

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id_usuario")]
    public short IdUsuario { get; set; }
    [Column("activo")]
    public bool Activo { get; set; }

    [Required]
    [StringLength(100)]
    [Column("correo")]
    public string Correo { get; set; }

    [Required]
    [StringLength(20)]
    [Column("telefono")]
    public string Telefono { get; set; }

    [Required]
    [StringLength(65)]
    [Column("contrasenia")]
    public string Contrasenia { get; set; }

    [Required]
    [StringLength(50)]
    [Column("identificacion")]
    public string Identificacion { get; set; }

    [Required]
    [StringLength(50)]
    [Column("nombre")]
    public string Nombre { get; set; }

    [Required]
    [StringLength(50)]
    [Column("apellido")]
    public string Apellido { get; set; }

    [Column("fecha_alta", TypeName = "date")]
    public DateTime FechaAlta { get; set; }
    public ICollection<UsuarioPermiso> UsuariosPermisos { get; set; } = new List<UsuarioPermiso>();
    public ICollection<UsuarioGrupoPermisos> UsuariosGruposPermisos { get; set; } = new List<UsuarioGrupoPermisos>();
}