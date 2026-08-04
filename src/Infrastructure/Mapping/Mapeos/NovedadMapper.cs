using Riok.Mapperly.Abstractions;
using EF = src.Models.CodeFirst;
using Dom = src.Models.Domain;
using src.Models.CodeFirst;

namespace src.Models.Mappers;

public static partial class DominioMapper
{
    [MapperIgnoreSource(nameof(Contracts.ProductoProveedorDataRow.StockActual))]
    [MapperIgnoreTarget(nameof(Contracts.NovedadPendiente.IdNovedad))]
    [MapperIgnoreTarget(nameof(Contracts.NovedadPendiente.IdProveedor))]
    [MapperIgnoreTarget(nameof(Contracts.NovedadPendiente.Estado))]
    [MapProperty(nameof(Contracts.ProductoProveedorDataRow.Precio), nameof(Contracts.NovedadPendiente.PrecioSugerido))]
    public static partial Contracts.NovedadPendiente MapDataRow(Contracts.ProductoProveedorDataRow source);


}