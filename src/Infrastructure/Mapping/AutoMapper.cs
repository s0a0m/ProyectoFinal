using Riok.Mapperly.Abstractions;
using EF = src.Models.CodeFirst;
using Dom = src.Models.Domain;
using src.Models.CodeFirst;

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
    [MapperIgnoreSource(nameof(EF.Proveedor.ProductosProveedores))]
    public static partial Dom.Proveedor Map(EF.Proveedor source);
    public static partial IEnumerable<Dom.Proveedor> Map(IEnumerable<EF.Proveedor> source);

    [MapperIgnoreSource(nameof(EF.Permiso.UsuariosPermisos))]
    [MapperIgnoreSource(nameof(EF.Permiso.GruposPermisosPermisos))]
    public static partial Dom.Permiso Map(EF.Permiso source);
    public static partial IEnumerable<Dom.Permiso> Map(IEnumerable<EF.Permiso> source);
    [MapperIgnoreSource(nameof(EF.Usuario.UsuariosPermisos))]
    [MapperIgnoreSource(nameof(EF.Usuario.UsuariosGruposPermisos))]
    [MapperIgnoreTarget(nameof(Dom.Usuario.Permisos))]
    public static partial Dom.Usuario Map(EF.Usuario source);
    public static partial IEnumerable<Dom.Usuario> Map(IEnumerable<EF.Usuario> source);

    [MapperIgnoreSource(nameof(EF.GrupoPermisos.GruposPermisosPermisos))]
    [MapperIgnoreSource(nameof(EF.GrupoPermisos.UsuariosGruposPermisos))]
    public static partial Dom.GrupoPermisos Map(EF.GrupoPermisos source);
    public static partial IEnumerable<Dom.GrupoPermisos> Map(IEnumerable<EF.GrupoPermisos> source);


    // --- Domain -> EF (CodeFirst) [BIDIRECCIONAL] ---

    [MapperIgnoreSource(nameof(Dom.GrupoPermisos.Permisos))]
    [MapperIgnoreTarget(nameof(EF.GrupoPermisos.GruposPermisosPermisos))]
    [MapperIgnoreTarget(nameof(EF.GrupoPermisos.UsuariosGruposPermisos))]
    public static partial EF.GrupoPermisos Map(Dom.GrupoPermisos source);

    public static partial IEnumerable<EF.GrupoPermisos> Map(IEnumerable<Dom.GrupoPermisos> source);

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
    [MapperIgnoreTarget(nameof(EF.Proveedor.ProductosProveedores))]
    public static partial EF.Proveedor Map(Dom.Proveedor source);
    public static partial IEnumerable<EF.Proveedor> Map(IEnumerable<Dom.Proveedor> source);
    [MapperIgnoreSource(nameof(Dom.Usuario.Permisos))]
    [MapperIgnoreTarget(nameof(EF.Usuario.UsuariosPermisos))]
    [MapperIgnoreTarget(nameof(EF.Usuario.UsuariosGruposPermisos))]
    public static partial EF.Usuario Map(Dom.Usuario source);
    public static partial IEnumerable<EF.Usuario> Map(IEnumerable<Dom.Usuario> source);
    [MapperIgnoreTarget(nameof(EF.Permiso.UsuariosPermisos))]
    [MapperIgnoreTarget(nameof(EF.Permiso.GruposPermisosPermisos))]
    public static partial EF.Permiso Map(Dom.Permiso source);
    public static partial IEnumerable<EF.Permiso> Map(IEnumerable<Dom.Permiso> source);


    // nuevas it 3

    // Categoria (NUEVO)
    public static partial Dom.Categoria Map(EF.Categoria source);

    // Grupo (NUEVO)
    public static partial Dom.Grupo Map(EF.Grupo source);

    // CodigoBarra (NUEVO)
    public static partial Dom.CodigoBarra Map(EF.CodigoBarra source);

    // ProductoProveedor (NUEVO)
    [MapperIgnoreSource(nameof(EF.ProductoProveedor.IdProducto))]
    [MapperIgnoreSource(nameof(EF.ProductoProveedor.IdProveedor))]
    [MapperIgnoreSource(nameof(EF.ProductoProveedor.Producto))]
    [MapperIgnoreSource(nameof(EF.ProductoProveedor.Proveedor))]
    public static partial Dom.ProductoProveedor Map(EF.ProductoProveedor source);

    // Producto (NUEVO)
    [MapProperty(nameof(EF.Producto.Grupo), nameof(Dom.Producto.Grupo))]
    [MapProperty(nameof(EF.Producto.Categoria), nameof(Dom.Producto.Categoria))]
    [MapProperty(nameof(EF.Producto.CodigoBarra), nameof(Dom.Producto.CodigoBarra))]
    [MapProperty(nameof(EF.Producto.ProductosProveedores), nameof(Dom.Producto.RelacionesProveedor))]
    [MapperIgnoreSource(nameof(EF.Producto.IdGrupo))]
    [MapperIgnoreSource(nameof(EF.Producto.IdCategoria))]
    [MapperIgnoreSource(nameof(EF.Producto.IdCodigoBarra))]
    public static partial Dom.Producto Map(EF.Producto source);


    // Categoria (NUEVO)
    public static partial EF.Categoria Map(Dom.Categoria source);

    // Grupo (NUEVO)
    public static partial EF.Grupo Map(Dom.Grupo source);

    // CodigoBarra (NUEVO)
    public static partial EF.CodigoBarra Map(Dom.CodigoBarra source);

    // ProductoProveedor (NUEVO)
    [MapProperty(nameof(Dom.ProductoProveedor.Producto.IdProducto), nameof(EF.ProductoProveedor.IdProducto))]
    [MapProperty(nameof(Dom.ProductoProveedor.Proveedor.IdProveedor), nameof(EF.ProductoProveedor.IdProveedor))]
    [MapperIgnoreTarget(nameof(EF.ProductoProveedor.Producto))]
    [MapperIgnoreTarget(nameof(EF.ProductoProveedor.Proveedor))]
    public static partial EF.ProductoProveedor Map(Dom.ProductoProveedor source);

    // Producto (NUEVO)
    [MapProperty(nameof(Dom.Producto.Grupo.IdGrupo), nameof(EF.Producto.IdGrupo))]
    [MapProperty(nameof(Dom.Producto.Categoria.IdCategoria), nameof(EF.Producto.IdCategoria))]
    [MapProperty(nameof(Dom.Producto.CodigoBarra.IdCodigoBarra), nameof(EF.Producto.IdCodigoBarra))]
    [MapperIgnoreTarget(nameof(EF.Producto.Grupo))]
    [MapperIgnoreTarget(nameof(EF.Producto.Categoria))]
    [MapperIgnoreTarget(nameof(EF.Producto.CodigoBarra))]
    [MapProperty(nameof(Dom.Producto.RelacionesProveedor), nameof(EF.Producto.ProductosProveedores))]
    public static partial EF.Producto Map(Dom.Producto source);

}