using Riok.Mapperly.Abstractions;
using EF = src.Models.CodeFirst;
using Dom = src.Models.Domain;

namespace src.Models.Mappers;

[Mapper]
public partial class ProvinciaMapper
{
    [MapperIgnoreSource(nameof(EF.Provincia.Domicilios))]
    public partial Dom.Provincia ToDomain(EF.Provincia source);
    public partial IEnumerable<Dom.Provincia> ToDomain(IEnumerable<EF.Provincia> source);

    [MapperIgnoreTarget(nameof(EF.Provincia.Domicilios))]
    public partial EF.Provincia ToEntity(Dom.Provincia source);
    public partial IEnumerable<EF.Provincia> ToEntity(IEnumerable<Dom.Provincia> source);
}