using Riok.Mapperly.Abstractions;
using EF = src.Models.CodeFirst;
using Dom = src.Models.Domain;

namespace src.Models.Mappers;

public static partial class DominioMapper
{
    [MapperIgnoreSource(nameof(EF.Domicilio.Proveedores))]
    [MapProperty(nameof(EF.Domicilio.IdProvincia), nameof(Dom.Direccion.Prov.IdProvincia))]
    [MapProperty(nameof(EF.Domicilio.IdProvinciaNavigation), nameof(Dom.Direccion.Prov))]
    public static partial Dom.Direccion Map(EF.Domicilio source);
    public static partial IEnumerable<Dom.Direccion> Map(IEnumerable<EF.Domicilio> source);

    [MapperIgnoreTarget(nameof(EF.Domicilio.Proveedores))]
    [MapProperty(nameof(Dom.Direccion.Prov.IdProvincia), nameof(EF.Domicilio.IdProvincia))]
    // [MapProperty(nameof(Dom.Direccion.Prov), nameof(EF.Domicilio.IdProvinciaNavigation))]
    [MapperIgnoreTarget(nameof(EF.Domicilio.IdProvinciaNavigation))]
    public static partial EF.Domicilio Map(Dom.Direccion source);
    public static partial IEnumerable<EF.Domicilio> Map(IEnumerable<Dom.Direccion> source);

}