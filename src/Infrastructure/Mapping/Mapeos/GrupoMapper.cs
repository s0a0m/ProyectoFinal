using Riok.Mapperly.Abstractions;
using EF = src.Models.CodeFirst;
using Dom = src.Models.Domain;

namespace src.Models.Mappers;

public static partial class DominioMapper
{
    [MapperIgnoreSource(nameof(EF.Grupo.ProductosGrupos))]
    public static partial Dom.Grupo Map(EF.Grupo source);
    [MapperIgnoreTarget(nameof(EF.Grupo.ProductosGrupos))]
    public static partial EF.Grupo Map(Dom.Grupo source);
}