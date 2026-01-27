using Riok.Mapperly.Abstractions;
using EF = src.Models.CodeFirst;
using Dom = src.Models.Domain;

namespace src.Models.Mappers;

[Mapper]
public partial class CodigoBarraMapper
{
    [MapperIgnoreSource(nameof(EF.CodigoBarra.ProductosCodigosBarras))]
    public partial Dom.CodigoBarra ToDomain(EF.CodigoBarra source);
    public partial IEnumerable<Dom.CodigoBarra> ToDomain(IEnumerable<EF.CodigoBarra> source);
    [MapperIgnoreTarget(nameof(EF.CodigoBarra.ProductosCodigosBarras))]
    public partial EF.CodigoBarra ToEntity(Dom.CodigoBarra source);
    public partial IEnumerable<EF.CodigoBarra> ToEntityList(IEnumerable<Dom.CodigoBarra> source);
}