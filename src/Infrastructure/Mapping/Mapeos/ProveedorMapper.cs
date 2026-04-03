using Riok.Mapperly.Abstractions;
using EF = src.Models.CodeFirst;
using Dom = src.Models.Domain;

namespace src.Models.Mappers;

[Mapper]
[UseMapper(typeof(DomicilioMapper))]
public partial class ProveedorMapper
{
    private readonly DomicilioMapper _domicilioMapper;

    public ProveedorMapper(DomicilioMapper domicilioMapper)
    {
        _domicilioMapper = domicilioMapper;
    }

    [MapProperty(nameof(EF.Proveedor.IdCondicionPagoHabitualNavigation), nameof(Dom.Proveedor.Condicion))]
    [MapProperty(nameof(EF.Proveedor.IdDomicilioNavigation), nameof(Dom.Proveedor.Direccion))]
    [MapProperty(nameof(EF.Proveedor.IdDomicilio), nameof(Dom.Proveedor.Direccion.IdDomicilio))]
    [MapProperty(nameof(EF.Proveedor.IdCondicionPagoHabitual), nameof(Dom.Proveedor.Condicion.IdCondicionPago))]
    [MapperIgnoreSource(nameof(EF.Proveedor.ProductosProveedores))]
    public partial Dom.Proveedor ToDomain(EF.Proveedor source);
    public partial IEnumerable<Dom.Proveedor> ToDomain(IEnumerable<EF.Proveedor> source);

    [MapProperty(nameof(Dom.Proveedor.Condicion), nameof(EF.Proveedor.IdCondicionPagoHabitualNavigation))]
    [MapProperty(nameof(Dom.Proveedor.Direccion), nameof(EF.Proveedor.IdDomicilioNavigation))]
    [MapProperty(nameof(Dom.Proveedor.Condicion.IdCondicionPago), nameof(EF.Proveedor.IdCondicionPagoHabitual))]
    [MapProperty(nameof(Dom.Proveedor.Direccion.IdDomicilio), nameof(EF.Proveedor.IdDomicilio))]
    [MapperIgnoreTarget(nameof(EF.Proveedor.ProductosProveedores))]
    public partial EF.Proveedor ToEntity(Dom.Proveedor source);
    public partial IEnumerable<EF.Proveedor> ToEntity(IEnumerable<Dom.Proveedor> source);
}
