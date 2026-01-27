using Riok.Mapperly.Abstractions;
using EF = src.Models.CodeFirst;
using Dom = src.Models.Domain;

namespace src.Models.Mappers;

[Mapper]
public partial class FamiliaMapper
{
    public partial Dom.Familia ToDomain(EF.Familia source);
    public partial IEnumerable<Dom.Familia> ToDomain(IEnumerable<EF.Familia> source);
    [MapperIgnoreSource(nameof(EF.Familia.Categorias))]
    [MapperIgnoreTarget(nameof(EF.Familia.Categorias))]
    public partial EF.Familia ToEntity(Dom.Familia source);
    public partial IEnumerable<EF.Familia> ToEntity(IEnumerable<Dom.Familia> source);

}