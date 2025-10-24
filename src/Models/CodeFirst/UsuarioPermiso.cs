using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace src.Models.CodeFirst;

[Table("usuario_permiso")]
public class UsuarioPermiso
{
    public short IdUsuario { get; set; }
    public int IdPermiso { get; set; }
    public Usuario? Usuario { get; set; }
    public Permiso? Permiso { get; set; }
}
