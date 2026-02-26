using System.ComponentModel.DataAnnotations.Schema;

namespace src.Models.CodeFirst
{
    [Table("grupo_permiso_permiso")]
    public class GrupoPermisoPermiso
    {
        [Column("id_grupo_permiso")]
        public short IdGrupoPermiso { get; set; }

        [Column("id_permiso")]
        public int IdPermiso { get; set; }
        public GrupoPermisos GrupoPermiso { get; set; }
        public Permiso Permiso { get; set; }
    }
}