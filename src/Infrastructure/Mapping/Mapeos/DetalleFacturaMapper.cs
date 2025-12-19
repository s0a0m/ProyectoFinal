using Riok.Mapperly.Abstractions;
using EF = src.Models.CodeFirst;
using Dom = src.Models.Domain;

namespace src.Models.Mappers;

public static partial class DominioMapper
{
    [MapperIgnoreSource(nameof(EF.DetalleFactura.Factura))]
    [MapperIgnoreSource(nameof(EF.DetalleFactura.IdFactura))]
    [MapperIgnoreSource(nameof(EF.DetalleFactura.IdProducto))]
    [MapperIgnoreSource(nameof(EF.DetalleFactura.PrecioBruto))]
    [MapperIgnoreSource(nameof(EF.DetalleFactura.PorcentajeDescuento))]
    [MapperIgnoreSource(nameof(EF.DetalleFactura.Factura))]
    [MapperIgnoreTarget(nameof(Dom.DetalleFactura.Subtotal))]
    [MapProperty(nameof(EF.DetalleFactura.PrecioNeto), nameof(Dom.DetalleFactura.PrecioUnitario))]
    public static partial Dom.DetalleFactura Map(EF.DetalleFactura source);
    public static partial IEnumerable<Dom.DetalleFactura> Map(IEnumerable<EF.DetalleFactura> source);
    [MapperIgnoreTarget(nameof(EF.DetalleFactura.Factura))]
    [MapProperty(nameof(Dom.DetalleFactura.PrecioUnitario), nameof(EF.DetalleFactura.PrecioNeto))]
    [MapProperty(nameof(Dom.DetalleFactura.PrecioUnitario), nameof(EF.DetalleFactura.PrecioBruto))]
    public static partial EF.DetalleFactura Map(Dom.DetalleFactura source);
    public static partial IEnumerable<EF.DetalleFactura> Map(IEnumerable<Dom.DetalleFactura> source);
}