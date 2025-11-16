using Riok.Mapperly.Abstractions;
using EF = src.Models.CodeFirst;
using Dom = src.Models.Domain;

namespace src.Models.Mappers;

public static partial class DominioMapper
{
    [MapperIgnoreSource(nameof(EF.Categoria.ProductosCategorias))]
    public static partial Dom.Categoria Map(EF.Categoria source);
    [MapperIgnoreTarget(nameof(EF.Categoria.ProductosCategorias))]
    public static partial EF.Categoria Map(Dom.Categoria source);
}