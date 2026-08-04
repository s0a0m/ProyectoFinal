using System.ComponentModel.DataAnnotations.Schema;

namespace src.Models.CodeFirst
{
    [Table("usuario_grupo_permisos")]
    public class UsuarioGrupoPermisos
    {
        [Column("id_usuario")]
        public short IdUsuario { get; set; }

        [Column("id_grupo_permiso")]
        public short IdGrupoPermiso { get; set; }
        public Usuario Usuario { get; set; }
        public GrupoPermisos GrupoPermiso { get; set; }
    }
}