using Riok.Mapperly.Abstractions;
using EF = src.Models.CodeFirst;
using Dom = src.Models.Domain;

namespace src.Models.Mappers;

public static partial class DominioMapper
{

    [MapperIgnoreSource(nameof(EF.Permiso.UsuariosPermisos))]
    [MapperIgnoreSource(nameof(EF.Permiso.GruposPermisosPermisos))]
    public static partial Dom.Permiso Map(EF.Permiso source);
    public static partial IEnumerable<Dom.Permiso> Map(IEnumerable<EF.Permiso> source);

    [MapperIgnoreTarget(nameof(EF.Permiso.UsuariosPermisos))]
    [MapperIgnoreTarget(nameof(EF.Permiso.GruposPermisosPermisos))]
    public static partial EF.Permiso Map(Dom.Permiso source);
    public static partial IEnumerable<EF.Permiso> Map(IEnumerable<Dom.Permiso> source);
}