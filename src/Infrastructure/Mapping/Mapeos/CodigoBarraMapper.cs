using Riok.Mapperly.Abstractions;
using EF = src.Models.CodeFirst;
using Dom = src.Models.Domain;

namespace src.Models.Mappers;

public static partial class DominioMapper
{
    [MapperIgnoreSource(nameof(EF.CodigoBarra.ProductosCodigosBarras))]
    public static partial Dom.CodigoBarra Map(EF.CodigoBarra source);
    [MapperIgnoreTarget(nameof(EF.CodigoBarra.ProductosCodigosBarras))]
    public static partial EF.CodigoBarra Map(Dom.CodigoBarra source);
}