using Riok.Mapperly.Abstractions;
using EF = src.Models.CodeFirst;
using Dom = src.Models.Domain;

namespace src.Models.Mappers;

public static partial class DominioMapper
{
    [MapperIgnoreSource(nameof(EF.ProductoProveedor.Producto))]
    [MapperIgnoreSource(nameof(EF.ProductoProveedor.IdProducto))]
    [MapperIgnoreTarget(nameof(Dom.ProductoProveedor.Producto))]
    [MapProperty(nameof(EF.ProductoProveedor.Proveedor), nameof(Dom.ProductoProveedor.Proveedor))]
    [MapProperty(nameof(EF.ProductoProveedor.IdProveedor), nameof(Dom.ProductoProveedor.Proveedor.IdProveedor))]
    public static partial Dom.ProductoProveedor Map(EF.ProductoProveedor source);
    public static partial IEnumerable<Dom.ProductoProveedor> Map(IEnumerable<EF.ProductoProveedor> source);

    [MapProperty(nameof(Dom.ProductoProveedor.Producto.IdProducto), nameof(EF.ProductoProveedor.IdProducto))]
    [MapProperty(nameof(Dom.ProductoProveedor.Proveedor.IdProveedor), nameof(EF.ProductoProveedor.IdProveedor))]
    [MapperIgnoreTarget(nameof(EF.ProductoProveedor.Producto))]
    [MapperIgnoreTarget(nameof(EF.ProductoProveedor.Proveedor))]
    public static partial EF.ProductoProveedor Map(Dom.ProductoProveedor source);
    public static partial IEnumerable<EF.ProductoProveedor> Map(IEnumerable<Dom.ProductoProveedor> source);
}