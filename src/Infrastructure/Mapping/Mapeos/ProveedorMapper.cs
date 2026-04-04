using Riok.Mapperly.Abstractions;
using Dom = src.Models.Domain;
using EF = src.Models.CodeFirst;

namespace src.Models.Mappers;

[Mapper]
public partial class ProveedorMapper
{
    private readonly DomicilioMapper _domicilioMapper;

    public ProveedorMapper(DomicilioMapper domicilioMapper)
    {
        _domicilioMapper = domicilioMapper;
    }

    [MapProperty(
        nameof(EF.Proveedor.IdCondicionPagoHabitualNavigation),
        nameof(Dom.Proveedor.Condicion)
    )]
    [MapProperty(nameof(EF.Proveedor.IdDomicilioNavigation), nameof(Dom.Proveedor.Direccion))]
    [MapperIgnoreSource(nameof(EF.Proveedor.ProductosProveedores))]
    [MapperIgnoreSource(nameof(EF.Proveedor.CodigosBarrasExternos))]
    [MapperIgnoreSource(nameof(EF.Proveedor.IdDomicilio))]
    [MapperIgnoreSource(nameof(EF.Proveedor.IdCondicionPagoHabitual))]
    public partial Dom.Proveedor ToDomain(EF.Proveedor source);

    [MapDerivedType(typeof(EF.Contado), typeof(Dom.Contado))]
    [MapDerivedType(typeof(EF.Cuota), typeof(Dom.Cuota))]
    public partial Dom.CondicionDePago MapCondicion(EF.CondicionDePago source);

    [MapProperty(
        nameof(Dom.Proveedor.Condicion),
        nameof(EF.Proveedor.IdCondicionPagoHabitualNavigation)
    )]
    [MapProperty(nameof(Dom.Proveedor.Direccion), nameof(EF.Proveedor.IdDomicilioNavigation))]
    [MapperIgnoreTarget(nameof(EF.Proveedor.ProductosProveedores))]
    [MapperIgnoreTarget(nameof(EF.Proveedor.CodigosBarrasExternos))]
    [MapperIgnoreTarget(nameof(EF.Proveedor.IdDomicilio))]
    [MapperIgnoreTarget(nameof(EF.Proveedor.IdCondicionPagoHabitual))]
    public partial EF.Proveedor ToEntity(Dom.Proveedor source);

    [MapDerivedType(typeof(Dom.Contado), typeof(EF.Contado))]
    [MapDerivedType(typeof(Dom.Cuota), typeof(EF.Cuota))]
    public partial EF.CondicionDePago MapCondicionToEf(Dom.CondicionDePago source);

    public partial IEnumerable<Dom.Proveedor> ToDomain(IEnumerable<EF.Proveedor> source);

    private Dom.Direccion Map(EF.Domicilio source) => _domicilioMapper.ToDomain(source);

    private EF.Domicilio Map(Dom.Direccion source) => _domicilioMapper.ToEntity(source);
}
