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
}
