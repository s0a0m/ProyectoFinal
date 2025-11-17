using Riok.Mapperly.Abstractions;
using EF = src.Models.CodeFirst;
using Dom = src.Models.Domain;

namespace src.Models.Mappers;

public static partial class DominioMapper
{
    public static partial Dom.Familia Map(EF.Familia source);
    public static partial IEnumerable<Dom.Familia> Map(IEnumerable<EF.Familia> source);
    [MapperIgnoreSource(nameof(EF.Familia.Categorias))]
    [MapperIgnoreTarget(nameof(EF.Familia.Categorias))]
    public static partial EF.Familia Map(Dom.Familia source);
    public static partial IEnumerable<EF.Familia> Map(IEnumerable<Dom.Familia> source);

}