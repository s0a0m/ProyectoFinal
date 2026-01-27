using Riok.Mapperly.Abstractions;
using EF = src.Models.CodeFirst;
using Dom = src.Models.Domain;

namespace src.Models.Mappers;

[Mapper]
public partial class CategoriaMapper
{
    private readonly FamiliaMapper _familiaMapper;
    public CategoriaMapper(FamiliaMapper familiaMapper)
    {
        _familiaMapper = familiaMapper;
    }
    [MapperIgnoreSource(nameof(EF.Categoria.ProductosCategorias))]
    // [MapperIgnoreSource(nameof(EF.Categoria.Familia))]
    // [MapperIgnoreTarget(nameof(Dom.Categoria.Familia))]
    [MapperIgnoreSource(nameof(EF.Categoria.IdFamilia))]
    public partial Dom.Categoria ToDomain(EF.Categoria source);
    public partial IEnumerable<Dom.Categoria> ToDomain(IEnumerable<EF.Categoria> source);

    [MapperIgnoreTarget(nameof(EF.Categoria.ProductosCategorias))]
    [MapProperty(nameof(Dom.Categoria.Familia.IdFamilia), nameof(EF.Categoria.IdFamilia))]
    [MapperIgnoreTarget(nameof(Dom.Categoria.Familia))]
    public partial EF.Categoria ToEntity(Dom.Categoria source);
    public partial IEnumerable<EF.Categoria> ToEntity(IEnumerable<Dom.Categoria> source);
    private Dom.Familia MapToFamilia(EF.Familia source)
            => _familiaMapper.ToDomain(source);
}