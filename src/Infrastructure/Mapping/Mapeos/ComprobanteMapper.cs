using Riok.Mapperly.Abstractions;
using EF = src.Models.CodeFirst;
using Dom = src.Models.Domain;

namespace src.Models.Mappers;

public static partial class DominioMapper
{
    [MapDerivedType(typeof(EF.NotaCredito), typeof(Dom.NotaCredito))]
    [MapDerivedType(typeof(EF.NotaDebito), typeof(Dom.NotaDebito))]
    [MapperIgnoreSource(nameof(EF.NotaCredito.IdFacturaReferencia))]
    [MapperIgnoreSource(nameof(EF.NotaDebito.IdFacturaReferencia))]
    [MapperIgnoreSource(nameof(EF.Comprobante.IdProveedor))]
    [MapperIgnoreSource(nameof(EF.Comprobante.Proveedor))]
    [MapperIgnoreTarget(nameof(Dom.Comprobante.Proveedor))]
    // [MapProperty(nameof(EF.Comprobante.IdProveedor), nameof(Dom.Comprobante.Proveedor.IdProveedor))]
    [MapperIgnoreSource(nameof(EF.Comprobante.IdMotivo))]
    [MapperIgnoreSource(nameof(EF.Comprobante.IdCondicionPagoUsada))]
    public static partial Dom.Comprobante Map(EF.Comprobante source);
    public static partial IEnumerable<Dom.Comprobante> Map(IEnumerable<EF.Comprobante> source);

    [MapDerivedType(typeof(Dom.NotaCredito), typeof(EF.NotaCredito))]
    [MapDerivedType(typeof(Dom.NotaDebito), typeof(EF.NotaDebito))]
    [MapperIgnoreTarget(nameof(EF.NotaCredito.IdFacturaReferencia))]
    [MapperIgnoreTarget(nameof(EF.NotaDebito.IdFacturaReferencia))]
    [MapperIgnoreTarget(nameof(EF.Comprobante.IdProveedor))]
    [MapperIgnoreTarget(nameof(EF.Comprobante.IdMotivo))]
    [MapperIgnoreTarget(nameof(EF.Comprobante.IdCondicionPagoUsada))]
    public static partial EF.Comprobante Map(Dom.Comprobante source);
    public static partial IEnumerable<EF.Comprobante> Map(IEnumerable<Dom.Comprobante> source);

    // mapeos especificos

    public static partial IEnumerable<Dom.NotaCredito> Map(IEnumerable<EF.NotaCredito> source);
    public static partial IEnumerable<Dom.NotaDebito> Map(IEnumerable<EF.NotaDebito> source);

}