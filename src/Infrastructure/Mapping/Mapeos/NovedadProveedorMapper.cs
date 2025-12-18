using Riok.Mapperly.Abstractions;
using EF = src.Models.CodeFirst;
using Dom = src.Models.Domain;
using src.Contracts;

namespace src.Models.Mappers;

public static partial class DominioMapper
{
    [MapperIgnoreSource(nameof(EF.NovedadesProveedor.Proveedor))]
    [MapperIgnoreSource(nameof(EF.NovedadesProveedor.FechaImportacion))]
    public static partial NovedadPendiente Map(EF.NovedadesProveedor source);
    public static partial IEnumerable<NovedadPendiente> Map(IEnumerable<EF.NovedadesProveedor> source);

    [MapperIgnoreSource(nameof(NovedadPendiente.IdNovedad))]
    [MapperIgnoreTarget(nameof(EF.NovedadesProveedor.IdProducto))]
    [MapperIgnoreTarget(nameof(EF.NovedadesProveedor.FechaImportacion))]
    [MapperIgnoreTarget(nameof(EF.NovedadesProveedor.Proveedor))]
    [MapperIgnoreTarget(nameof(EF.NovedadesProveedor.Producto))]
    public static partial EF.NovedadesProveedor Map(NovedadPendiente source);
    public static partial IEnumerable<EF.NovedadesProveedor> Map(IEnumerable<NovedadPendiente> source);
}
