using Riok.Mapperly.Abstractions;
using EF = src.Models.CodeFirst;
using Dom = src.Models.Domain;

namespace src.Models.Mappers;

public static partial class DominioMapper
{
    [MapProperty(nameof(EF.Usuario.UsuariosPermisos), nameof(Dom.Usuario.PermisosUsuario))]
    [MapProperty(nameof(EF.Usuario.UsuariosGruposPermisos), nameof(Dom.Usuario.GrupoPermisos))]
    public static partial Dom.Usuario Map(EF.Usuario source);
    public static partial IEnumerable<Dom.Usuario> Map(IEnumerable<EF.Usuario> source);

    [MapperIgnoreSource(nameof(Dom.Usuario.PermisosUsuario))]
    [MapperIgnoreSource(nameof(Dom.Usuario.GrupoPermisos))]
    [MapperIgnoreTarget(nameof(EF.Usuario.UsuariosPermisos))]
    [MapperIgnoreTarget(nameof(EF.Usuario.UsuariosGruposPermisos))]
    public static partial EF.Usuario Map(Dom.Usuario source);
    public static partial IEnumerable<EF.Usuario> Map(IEnumerable<Dom.Usuario> source);

    public static Dom.Permiso Map(EF.UsuarioPermiso source)
    {
        if (source.Permiso == null)
        {
            return new Dom.Permiso();
        }
        return Map(source.Permiso);
    }
    public static Dom.GrupoPermisos Map(EF.UsuarioGrupoPermisos source)
    {
        if (source.GrupoPermiso == null)
        {
            return new Dom.GrupoPermisos();
        }
        return Map(source.GrupoPermiso);
    }
}