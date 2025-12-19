using Riok.Mapperly.Abstractions;
using EF = src.Models.CodeFirst;
using Dom = src.Models.Domain;

namespace src.Models.Mappers;

public static partial class DominioMapper
{
    [MapperIgnoreSource(nameof(EF.Factura.IdCompra))]
    [MapperIgnoreSource(nameof(EF.Factura.IdProveedor))]
    [MapperIgnoreSource(nameof(EF.Factura.IdCondicionPagoUsada))]
    // [MapperIgnoreSource(nameof(EF.Factura.Proveedor))]
    [MapperIgnoreSource(nameof(EF.Factura.Compra))]
    [MapperIgnoreSource(nameof(EF.Factura.CondicionPago))]

    // [MapperIgnoreTarget(nameof(Dom.Factura.Proveedor))]
    [MapperIgnoreTarget(nameof(Dom.Factura.Compra))]
    [MapperIgnoreTarget(nameof(Dom.Factura.CondicionPago))]
    public static partial Dom.Factura Map(EF.Factura source);
    public static partial IEnumerable<Dom.Factura> Map(IEnumerable<EF.Factura> source);
    public static partial EF.Factura Map(Dom.Factura source);
    public static partial IEnumerable<EF.Factura> Map(IEnumerable<Dom.Factura> source);
}