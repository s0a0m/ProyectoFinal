using Riok.Mapperly.Abstractions;
using EF = src.Models.CodeFirst;
using Dom = src.Models.Domain;

namespace src.Models.Mappers;

[Mapper]
public static partial class DominioMapper
{
    // --- EF (CodeFirst) -> Domain ---

    [MapDerivedType(typeof(EF.Contado), typeof(Dom.Contado))]
    [MapDerivedType(typeof(EF.Cuota), typeof(Dom.Cuota))]
    public static partial Dom.CondicionDePago Map(EF.CondicionDePago source);
    public static partial IEnumerable<Dom.CondicionDePago> Map(IEnumerable<EF.CondicionDePago> source);

    [MapperIgnoreSource(nameof(EF.Domicilio.Proveedores))]
    [MapperIgnoreSource(nameof(EF.Domicilio.IdProvincia))]
    [MapProperty(
            nameof(EF.Domicilio.IdProvinciaNavigation),
            nameof(Dom.Direccion.Prov)
        )]
    public static partial Dom.Direccion Map(EF.Domicilio source);
    public static partial IEnumerable<Dom.Direccion> Map(IEnumerable<EF.Domicilio> source);

    [MapperIgnoreSource(nameof(EF.Provincia.Domicilios))]
    public static partial Dom.Provincia Map(EF.Provincia source);
    public static partial IEnumerable<Dom.Provincia> Map(IEnumerable<EF.Provincia> source);

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
    public static partial IEnumerable<Dom.Proveedor> Map(IEnumerable<EF.Proveedor> source);

    [MapperIgnoreSource(nameof(EF.Permiso.UsuariosPermisos))]
    public static partial Dom.Permiso Map(EF.Permiso source);
    public static partial IEnumerable<Dom.Permiso> Map(IEnumerable<EF.Permiso> source);
    [MapperIgnoreSource(nameof(EF.Usuario.UsuariosPermisos))]
    public static partial Dom.Usuario Map(EF.Usuario source);
    public static partial IEnumerable<Dom.Usuario> Map(IEnumerable<EF.Usuario> source);


    // --- Domain -> EF (CodeFirst) [BIDIRECCIONAL] ---

    [MapDerivedType(typeof(Dom.Contado), typeof(EF.Contado))]
    [MapDerivedType(typeof(Dom.Cuota), typeof(EF.Cuota))]

    public static partial EF.CondicionDePago Map(Dom.CondicionDePago source);
    public static partial IEnumerable<EF.CondicionDePago> Map(IEnumerable<Dom.CondicionDePago> source);

    [MapperIgnoreTarget(nameof(EF.Domicilio.Proveedores))]
    [MapperIgnoreTarget(nameof(EF.Domicilio.IdProvinciaNavigation))] // <-- Ignora el objeto de navegación
    [MapProperty(
        nameof(Dom.Direccion.Prov.IdProvincia), // <-- Mapea DESDE el ID
        nameof(EF.Domicilio.IdProvincia)        // <-- Mapea HACIA la clave foránea
    )]
    public static partial EF.Domicilio Map(Dom.Direccion source);
    public static partial IEnumerable<EF.Domicilio> Map(IEnumerable<Dom.Direccion> source);

    [MapperIgnoreTarget(nameof(EF.Provincia.Domicilios))]
    public static partial EF.Provincia Map(Dom.Provincia source);
    public static partial IEnumerable<EF.Provincia> Map(IEnumerable<Dom.Provincia> source);

    [MapProperty(
            nameof(Dom.Proveedor.Condicion),
            nameof(EF.Proveedor.IdCondicionPagoHabitualNavigation)
        )]
    [MapProperty(
            nameof(Dom.Proveedor.Direccion),
            nameof(EF.Proveedor.IdDomicilioNavigation)
        )]
    [MapperIgnoreTarget(nameof(EF.Proveedor.IdCondicionPagoHabitual))]
    [MapperIgnoreTarget(nameof(EF.Proveedor.IdDomicilio))]
    public static partial EF.Proveedor Map(Dom.Proveedor source);
    public static partial IEnumerable<EF.Proveedor> Map(IEnumerable<Dom.Proveedor> source);
    [MapperIgnoreTarget(nameof(EF.Usuario.UsuariosPermisos))]
    public static partial EF.Usuario Map(Dom.Usuario source);
    public static partial IEnumerable<EF.Usuario> Map(IEnumerable<Dom.Usuario> source);
    [MapperIgnoreTarget(nameof(EF.Permiso.UsuariosPermisos))]
    public static partial EF.Permiso Map(Dom.Permiso source);
    public static partial IEnumerable<EF.Permiso> Map(IEnumerable<Dom.Permiso> source);

}