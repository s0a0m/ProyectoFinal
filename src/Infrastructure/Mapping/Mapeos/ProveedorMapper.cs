using Riok.Mapperly.Abstractions;
using EF = src.Models.CodeFirst;
using Dom = src.Models.Domain;

namespace src.Models.Mappers;

public static partial class DominioMapper
{
    [MapProperty(nameof(EF.Proveedor.IdCondicionPagoHabitualNavigation), nameof(Dom.Proveedor.Condicion))]
    [MapProperty(nameof(EF.Proveedor.IdDomicilioNavigation), nameof(Dom.Proveedor.Direccion))]
    [MapProperty(nameof(EF.Proveedor.IdDomicilio), nameof(Dom.Proveedor.Direccion.IdDomicilio))]
    [MapProperty(nameof(EF.Proveedor.IdCondicionPagoHabitual), nameof(Dom.Proveedor.Condicion.IdCondicionPago))]
    [MapperIgnoreSource(nameof(EF.Proveedor.ProductosProveedores))]
    public static partial Dom.Proveedor Map(EF.Proveedor source);
    public static partial IEnumerable<Dom.Proveedor> Map(IEnumerable<EF.Proveedor> source);

    [MapProperty(nameof(Dom.Proveedor.Condicion), nameof(EF.Proveedor.IdCondicionPagoHabitualNavigation))]
    [MapProperty(nameof(Dom.Proveedor.Direccion), nameof(EF.Proveedor.IdDomicilioNavigation))]
    [MapProperty(nameof(Dom.Proveedor.Condicion.IdCondicionPago), nameof(EF.Proveedor.IdCondicionPagoHabitual))]
    [MapProperty(nameof(Dom.Proveedor.Direccion.IdDomicilio), nameof(EF.Proveedor.IdDomicilio))]
    [MapperIgnoreTarget(nameof(EF.Proveedor.ProductosProveedores))]
    // [MapperIgnoreTarget(nameof(EF.Domicilio.IdProvinciaNavigation))]
    public static partial EF.Proveedor Map(Dom.Proveedor source);
    public static partial IEnumerable<EF.Proveedor> Map(IEnumerable<Dom.Proveedor> source);
}