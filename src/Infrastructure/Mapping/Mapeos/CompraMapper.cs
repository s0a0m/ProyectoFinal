using Riok.Mapperly.Abstractions;
using EF = src.Models.CodeFirst;
using Dom = src.Models.Domain;

namespace src.Models.Mappers;

public static partial class DominioMapper
{
    [MapperIgnoreSource(nameof(EF.Compra.IdProveedor))]
    [MapperIgnoreSource(nameof(EF.Compra.IdUsuario))]
    [MapperIgnoreSource(nameof(EF.Compra.Facturas))]
    public static partial Dom.Compra Map(EF.Compra source);
    public static partial IEnumerable<Dom.Compra> Map(IEnumerable<EF.Compra> source);
    [MapperIgnoreTarget(nameof(EF.Compra.IdProveedor))]
    [MapperIgnoreTarget(nameof(EF.Compra.IdUsuario))]
    [MapperIgnoreTarget(nameof(EF.Compra.Facturas))]
    [MapperIgnoreTarget(nameof(EF.Compra.Usuario))]
    [MapperIgnoreTarget(nameof(EF.Compra.Proveedor))]

    [MapperIgnoreSource(nameof(Dom.Compra.Proveedor))]
    [MapperIgnoreSource(nameof(Dom.Compra.Usuario))]
    [MapperIgnoreSource(nameof(Dom.Compra.TotalOrden))]

    public static partial EF.Compra Map(Dom.Compra source);
    public static partial IEnumerable<EF.Compra> Map(IEnumerable<Dom.Compra> source);
}