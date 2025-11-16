using Riok.Mapperly.Abstractions;
using EF = src.Models.CodeFirst;
using Dom = src.Models.Domain;

namespace src.Models.Mappers;

public static partial class DominioMapper
{
    [MapperIgnoreSource(nameof(EF.Provincia.Domicilios))]
    public static partial Dom.Provincia Map(EF.Provincia source);
    public static partial IEnumerable<Dom.Provincia> Map(IEnumerable<EF.Provincia> source);

    [MapperIgnoreTarget(nameof(EF.Provincia.Domicilios))]
    public static partial EF.Provincia Map(Dom.Provincia source);
    public static partial IEnumerable<EF.Provincia> Map(IEnumerable<Dom.Provincia> source);

}