using Riok.Mapperly.Abstractions;
using EF = src.Models.CodeFirst;
using Dom = src.Models.Domain;

namespace src.Models.Mappers;

public static partial class DominioMapper
{
    [MapperIgnoreSource(nameof(EF.DetalleCompra.IdCompra))]
    [MapperIgnoreSource(nameof(EF.DetalleCompra.IdProducto))]
    [MapperIgnoreSource(nameof(EF.DetalleCompra.Compra))]
    [MapperIgnoreTarget(nameof(Dom.DetalleCompra.Compra))]
    public static partial Dom.DetalleCompra Map(EF.DetalleCompra source);
    public static partial IEnumerable<Dom.DetalleCompra> Map(IEnumerable<EF.DetalleCompra> source);

    [MapperIgnoreSource(nameof(Dom.DetalleCompra.Subtotal))]
    [MapperIgnoreSource(nameof(Dom.DetalleCompra.Compra))]
    [MapperIgnoreSource(nameof(Dom.DetalleCompra.Producto))]
    [MapperIgnoreTarget(nameof(EF.DetalleCompra.IdCompra))]
    [MapperIgnoreTarget(nameof(EF.DetalleCompra.IdProducto))]
    [MapperIgnoreTarget(nameof(EF.DetalleCompra.Compra))]
    [MapperIgnoreTarget(nameof(EF.DetalleCompra.Producto))]
    public static partial EF.DetalleCompra Map(Dom.DetalleCompra source);
    public static partial IEnumerable<EF.DetalleCompra> Map(IEnumerable<Dom.DetalleCompra> source);
}