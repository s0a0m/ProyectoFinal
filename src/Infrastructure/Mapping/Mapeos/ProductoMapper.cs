using Riok.Mapperly.Abstractions;
using EF = src.Models.CodeFirst;
using Dom = src.Models.Domain;
using System.Collections.Generic;

namespace src.Models.Mappers;

public static partial class DominioMapper
{

    [MapperIgnoreSource(nameof(EF.Producto.ProductosProveedores))]
    [MapperIgnoreSource(nameof(EF.Producto.Novedades))]
    [MapProperty(nameof(EF.Producto.ProductoCodigoBarras), nameof(Dom.Producto.CodigoBarra))]
    [MapProperty(nameof(EF.Producto.ProductosCategorias), nameof(Dom.Producto.Categoria))]
    [MapperIgnoreSource(nameof(EF.Producto.CodigosBarrasExternos))]
    // [MapProperty(nameof(EF.Producto.ProductosGrupos), nameof(Dom.Producto.Grupo))]
    public static partial Dom.Producto Map(EF.Producto source);
    public static partial IEnumerable<Dom.Producto> Map(IEnumerable<EF.Producto> source);


    [MapperIgnoreTarget(nameof(EF.Producto.ProductosProveedores))]
    [MapperIgnoreTarget(nameof(EF.Producto.Novedades))]
    [MapperIgnoreTarget(nameof(EF.Producto.ProductoCodigoBarras))]
    [MapperIgnoreTarget(nameof(EF.Producto.ProductosCategorias))]
    // [MapperIgnoreTarget(nameof(EF.Producto.ProductosGrupos))]
    [MapperIgnoreTarget(nameof(EF.Producto.CodigosBarrasExternos))]

    // [MapperIgnoreSource(nameof(Dom.Producto.Grupo))]
    [MapperIgnoreSource(nameof(Dom.Producto.CodigoBarra))]
    [MapperIgnoreSource(nameof(Dom.Producto.Categoria))]
    public static partial EF.Producto Map(Dom.Producto source);
    public static partial IEnumerable<EF.Producto> Map(IEnumerable<Dom.Producto> source);

    public static Dom.CodigoBarra Map(EF.ProductoCodigoBarra source)
    {
        if (source.CodigoBarra == null)
        {
            return new Dom.CodigoBarra();
        }
        return Map(source.CodigoBarra);
    }
    public static Dom.Categoria Map(EF.ProductoCategoria source)
    {
        if (source.Categoria == null)
        {
            return new Dom.Categoria();
        }
        return Map(source.Categoria);
    }
    // public static Dom.Grupo Map(EF.ProductoGrupo source)
    // {
    //     if (source.Grupo == null)
    //     {
    //         return new Dom.Grupo();
    //     }
    //     return Map(source.Grupo);
    // }
}