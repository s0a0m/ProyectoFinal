using Riok.Mapperly.Abstractions;
using EF = src.Models.CodeFirst;
using Dom = src.Models.Domain;

namespace src.Models.Mappers;

public static partial class DominioMapper
{
    [MapProperty(nameof(EF.GrupoPermisos.GruposPermisosPermisos), nameof(Dom.GrupoPermisos.Permisos))]

    [MapperIgnoreSource(nameof(EF.GrupoPermisos.UsuariosGruposPermisos))]
    public static partial Dom.GrupoPermisos Map(EF.GrupoPermisos source);
    public static partial IEnumerable<Dom.GrupoPermisos> Map(IEnumerable<EF.GrupoPermisos> source);

    [MapperIgnoreSource(nameof(Dom.GrupoPermisos.Permisos))]
    [MapperIgnoreTarget(nameof(EF.GrupoPermisos.GruposPermisosPermisos))]
    [MapperIgnoreTarget(nameof(EF.GrupoPermisos.UsuariosGruposPermisos))]
    public static partial EF.GrupoPermisos Map(Dom.GrupoPermisos source);
    public static partial IEnumerable<EF.GrupoPermisos> Map(IEnumerable<Dom.GrupoPermisos> source);
    public static Dom.Permiso Map(EF.GrupoPermisoPermiso source)
    {
        if (source.Permiso == null)
        {
            return new Dom.Permiso();
        }
        return Map(source.Permiso);
    }
}
