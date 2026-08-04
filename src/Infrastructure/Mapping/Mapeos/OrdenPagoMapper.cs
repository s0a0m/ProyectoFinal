using Riok.Mapperly.Abstractions;
using EF = src.Models.CodeFirst;
using Dom = src.Models.Domain;

namespace src.Models.Mappers;

public static partial class DominioMapper
{
    // [MapperIgnoreSource(nameof(EF.OrdenPago.IdProveedor))]
    public static partial Dom.OrdenPago Map(EF.OrdenPago source);
    public static partial IEnumerable<Dom.OrdenPago> Map(IEnumerable<EF.OrdenPago> source);
    // [MapperIgnoreTarget(nameof(EF.OrdenPago.IdProveedor))]
    // [MapperIgnoreSource(nameof(EF.OrdenPago.IdProveedor))]
    [MapperIgnoreSource(nameof(Dom.OrdenPago.Proveedor))]
    [MapperIgnoreTarget(nameof(EF.OrdenPago.Proveedor))]
    public static partial EF.OrdenPago Map(Dom.OrdenPago source);
    public static partial IEnumerable<EF.OrdenPago> Map(IEnumerable<Dom.OrdenPago> source);

    // pago detalle 
    // [MapperIgnoreSource(nameof(EF.PagoDetalle.IdFactura))]
    [MapperIgnoreSource(nameof(EF.PagoDetalle.IdOrdenPago))]
    [MapperIgnoreSource(nameof(EF.PagoDetalle.OrdenPago))]
    public static partial Dom.PagoDetalle Map(EF.PagoDetalle source);
    public static partial IEnumerable<Dom.PagoDetalle> Map(IEnumerable<EF.PagoDetalle> source);
    // [MapperIgnoreTarget(nameof(EF.PagoDetalle.IdFactura))]
    [MapperIgnoreTarget(nameof(EF.PagoDetalle.IdOrdenPago))]
    [MapperIgnoreTarget(nameof(EF.PagoDetalle.OrdenPago))]
    // [MapperIgnoreTarget(nameof(EF.PagoDetalle.OrdenPago))]
    public static partial EF.PagoDetalle Map(Dom.PagoDetalle source);
    public static partial IEnumerable<EF.PagoDetalle> Map(IEnumerable<Dom.PagoDetalle> source);
}