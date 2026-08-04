using Riok.Mapperly.Abstractions;
using Dom = src.Models.Domain;
using EF = src.Models.CodeFirst;

namespace src.Models.Mappers;

[Mapper(UseReferenceHandling = true)]
public partial class ComprobanteMapper
{
    private readonly ProveedorMapper _proveedorMapper;

    public ComprobanteMapper(ProveedorMapper proveedorMapper)
    {
        _proveedorMapper = proveedorMapper;
    }

    // =============================================
    // --- Entity Framework a Dominio (interno) ---
    // =============================================
    [MapDerivedType(typeof(EF.NotaCredito), typeof(Dom.NotaCredito))]
    [MapDerivedType(typeof(EF.NotaDebito), typeof(Dom.NotaDebito))]
    [MapperIgnoreSource(nameof(EF.Comprobante.IdProveedor))]
    [MapperIgnoreSource(nameof(EF.Comprobante.IdMotivo))]
    [MapperIgnoreSource(nameof(EF.Comprobante.Proveedor))]
    [MapperIgnoreTarget(nameof(Dom.Comprobante.Proveedor))]
    [MapperIgnoreSource(nameof(EF.Comprobante.FacturaOriginal))]
    [MapperIgnoreTarget(nameof(Dom.Comprobante.FacturaOriginal))]
    private partial Dom.Comprobante ToDomainInternal(EF.Comprobante source);

    // =============================================
    // --- EF a Dominio (público con post-mapeo) ---
    // =============================================
    public Dom.Comprobante ToDomain(EF.Comprobante source)
    {
        var result = ToDomainInternal(source);

        if (source.Proveedor != null)
            result.Proveedor = _proveedorMapper.ToDomain(source.Proveedor);

        if (source.FacturaOriginal != null)
            result.FacturaOriginal = MapFacturaToDomain(source.FacturaOriginal);

        return result;
    }

    public IEnumerable<Dom.Comprobante> ToDomain(IEnumerable<EF.Comprobante> source) =>
        source.Select(ToDomain);

    public IEnumerable<Dom.NotaCredito> ToDomain(IEnumerable<EF.NotaCredito> source) =>
        source.Select(s => (Dom.NotaCredito)ToDomain(s));

    public IEnumerable<Dom.NotaDebito> ToDomain(IEnumerable<EF.NotaDebito> source) =>
        source.Select(s => (Dom.NotaDebito)ToDomain(s));

    // =============================================
    // --- Dominio a Entity Framework ---
    // =============================================
    [MapDerivedType(typeof(Dom.NotaCredito), typeof(EF.NotaCredito))]
    [MapDerivedType(typeof(Dom.NotaDebito), typeof(EF.NotaDebito))]
    [MapperIgnoreTarget(nameof(EF.Comprobante.IdProveedor))]
    [MapperIgnoreTarget(nameof(EF.Comprobante.IdMotivo))]
    [MapperIgnoreTarget(nameof(EF.Comprobante.IdFacturaReferencia))]
    [MapperIgnoreTarget(nameof(EF.Comprobante.Proveedor))]
    [MapperIgnoreTarget(nameof(EF.Comprobante.FacturaOriginal))]
    public partial EF.Comprobante ToEntity(Dom.Comprobante source);

    public partial IEnumerable<EF.Comprobante> ToEntity(IEnumerable<Dom.Comprobante> source);

    // =============================================
    // --- CondicionDePago ---
    // =============================================
    [MapDerivedType(typeof(Dom.Contado), typeof(EF.Contado))]
    [MapDerivedType(typeof(Dom.Cuota), typeof(EF.Cuota))]
    public partial EF.CondicionDePago MapCondicion(Dom.CondicionDePago source);

    [MapDerivedType(typeof(EF.Contado), typeof(Dom.Contado))]
    [MapDerivedType(typeof(EF.Cuota), typeof(Dom.Cuota))]
    public partial Dom.CondicionDePago MapCondicion(EF.CondicionDePago source);

    // =============================================
    // --- Helpers manuales ---
    // =============================================
    private Dom.Factura MapFacturaToDomain(EF.Factura source)
    {
        var factura = new Dom.Factura
        {
            IdFactura = source.IdFactura,
            Numero = source.Numero,
            FechaEmision = source.FechaEmision,
            FechaPago = source.FechaPago ?? default,
            TotalFacturado = source.TotalFacturado,
            Saldo = source.Saldo,
            Pagada = source.Pagada,
            IdCondicionPagoUsada = source.IdCondicionPagoUsada,
        };

        if (source.Proveedor != null)
            factura.Proveedor = _proveedorMapper.ToDomain(source.Proveedor);

        if (source.CondicionPago != null)
            factura.CondicionPago = MapCondicion(source.CondicionPago);

        return factura;
    }
}
