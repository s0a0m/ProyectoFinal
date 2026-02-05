using Riok.Mapperly.Abstractions;
using EF = src.Models.CodeFirst;
using Dom = src.Models.Domain;

namespace src.Models.Mappers;

[Mapper]
public partial class ProductoMapper
{
    private readonly CategoriaMapper _categoriaMapper;
    private readonly CodigoBarraMapper _codigoBarraMapper;
    public ProductoMapper(CategoriaMapper categoriaMapper, CodigoBarraMapper codigoBarraMapper)
    {
        _categoriaMapper = categoriaMapper;
        _codigoBarraMapper = codigoBarraMapper;
    }

    [MapperIgnoreSource(nameof(EF.Producto.ProductosProveedores))]
    [MapperIgnoreSource(nameof(EF.Producto.Novedades))]
    [MapperIgnoreSource(nameof(EF.Producto.CodigosBarrasExternos))]
    [MapProperty(nameof(EF.Producto.ProductoCodigoBarras), nameof(Dom.Producto.CodigoBarra))]
    [MapProperty(nameof(EF.Producto.ProductosCategorias), nameof(Dom.Producto.Categoria))]
    public partial Dom.Producto ToDomain(EF.Producto source);
    public partial IEnumerable<Dom.Producto> ToDomain(IEnumerable<EF.Producto> source);

    [MapperIgnoreTarget(nameof(EF.Producto.ProductosProveedores))]
    [MapperIgnoreTarget(nameof(EF.Producto.Novedades))]
    [MapperIgnoreTarget(nameof(EF.Producto.ProductoCodigoBarras))]
    [MapperIgnoreTarget(nameof(EF.Producto.ProductosCategorias))]
    [MapperIgnoreTarget(nameof(EF.Producto.CodigosBarrasExternos))]
    [MapperIgnoreSource(nameof(Dom.Producto.CodigoBarra))]
    [MapperIgnoreSource(nameof(Dom.Producto.Categoria))]
    public partial EF.Producto ToEntity(Dom.Producto source);

    public partial IEnumerable<EF.Producto> ToEntity(IEnumerable<Dom.Producto> source);

    private Dom.CodigoBarra MapToCodigoBarra(EF.ProductoCodigoBarra source)
    {
        if (source?.CodigoBarra == null) return null;
        return _codigoBarraMapper.ToDomain(source.CodigoBarra);
    }
    private Dom.Categoria MapToCategoria(EF.ProductoCategoria source)
    {
        if (source?.Categoria == null) return null;

        return _categoriaMapper.ToDomain(source.Categoria);
    }
}
