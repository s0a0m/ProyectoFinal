using Riok.Mapperly.Abstractions;
using Dom = src.Models.Domain;
using EF = src.Models.CodeFirst;

namespace src.Models.Mappers;

[Mapper]
public partial class DomicilioMapper
{
    private readonly ProvinciaMapper _provinciaMapper;

    public DomicilioMapper(ProvinciaMapper provinciaMapper) => _provinciaMapper = provinciaMapper;

    [MapProperty(nameof(EF.Domicilio.IdProvinciaNavigation), nameof(Dom.Direccion.Prov))]
    public partial Dom.Direccion ToDomain(EF.Domicilio source);

    public partial IEnumerable<Dom.Direccion> ToDomain(IEnumerable<EF.Domicilio> source);

    [MapProperty(nameof(Dom.Direccion.Prov.IdProvincia), nameof(EF.Domicilio.IdProvincia))]
    public partial EF.Domicilio ToEntity(Dom.Direccion source);

    public partial IEnumerable<EF.Domicilio> ToEntity(IEnumerable<Dom.Direccion> source);
}

