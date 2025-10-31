using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace src.Models.CodeFirst;

[Table("permiso")]
public class Permiso
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int IdPermiso { get; set; }

    [Required]
    [MaxLength(80)]
    public string Nombre { get; set; }

    [MaxLength(500)]
    public string Descripcion { get; set; }
    public ICollection<UsuarioPermiso> UsuariosPermisos { get; set; }
    public ICollection<GrupoPermisoPermiso> GruposPermisosPermisos { get; set; }
}
