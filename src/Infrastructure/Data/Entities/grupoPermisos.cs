using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace src.Models.CodeFirst
{
    [Table("grupo_permisos")]
    public class GrupoPermisos
    {
        [Key]
        [Column("id_grupo_permiso")]
        public short IdGrupoPermiso { get; set; }

        [Required]
        [StringLength(50)]
        [Column("nombre")]
        public string Nombre { get; set; }

        [StringLength(150)]
        [Column("descripcion")]
        public string Descripcion { get; set; }
        public ICollection<GrupoPermisoPermiso> GruposPermisosPermisos { get; set; } = new List<GrupoPermisoPermiso>();
        public ICollection<UsuarioGrupoPermisos> UsuariosGruposPermisos { get; set; } = new List<UsuarioGrupoPermisos>();
    }
}