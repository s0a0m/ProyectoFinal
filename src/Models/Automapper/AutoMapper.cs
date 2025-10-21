using Riok.Mapperly.Abstractions;
using EF = src.Models.CodeFirst;
using Dom = src.Models.Domain;

namespace src.Models.Mappers;

[Mapper]
public static partial class DominioMapper
{
    [MapDerivedType(typeof(EF.Contado), typeof(Dom.Contado))]
    [MapDerivedType(typeof(EF.Cuota), typeof(Dom.Cuota))]
    public static partial Dom.CondicionDePago Map(EF.CondicionDePago source);

    [MapperIgnoreSource(nameof(EF.Domicilio.IdProvinciaNavigation))]
    [MapperIgnoreSource(nameof(EF.Domicilio.Proveedores))]
    public static partial Dom.Direccion Map(EF.Domicilio source);

    [MapperIgnoreSource(nameof(EF.Provincia.Domicilios))]
    public static partial Dom.Provincia Map(EF.Provincia source);

    [MapProperty(
            nameof(EF.Proveedor.IdCondicionPagoHabitualNavigation),
            nameof(Dom.Proveedor.Condicion)
        )]
    [MapProperty(
            nameof(EF.Proveedor.IdDomicilioNavigation),
            nameof(Dom.Proveedor.Direccion)
        )]

    [MapperIgnoreSource(nameof(EF.Proveedor.IdCondicionPagoHabitual))]
    [MapperIgnoreSource(nameof(EF.Proveedor.IdDomicilio))]
    public static partial Dom.Proveedor Map(EF.Proveedor source);

    public static partial Dom.Usuario Map(EF.Usuario source);
}
