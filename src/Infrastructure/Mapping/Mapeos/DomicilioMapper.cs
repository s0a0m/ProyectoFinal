using Riok.Mapperly.Abstractions;
using EF = src.Models.CodeFirst;
using Dom = src.Models.Domain;

namespace src.Models.Mappers;

[Mapper]
public partial class DomicilioMapper
{
    private readonly ProvinciaMapper _provinciaMapper;
    public DomicilioMapper(ProvinciaMapper provinciaMapper)
    {
        _provinciaMapper = provinciaMapper;
    }

    [MapperIgnoreSource(nameof(EF.Domicilio.Proveedores))]
    [MapperIgnoreSource(nameof(EF.Domicilio.IdProvincia))]
    [MapProperty(nameof(EF.Domicilio.IdProvinciaNavigation), nameof(Dom.Direccion.Prov))]
    public partial Dom.Direccion ToDomain(EF.Domicilio source);
    public partial IEnumerable<Dom.Direccion> ToDomain(IEnumerable<EF.Domicilio> source);

    [MapperIgnoreTarget(nameof(EF.Domicilio.Proveedores))]
    [MapProperty(nameof(Dom.Direccion.Prov.IdProvincia), nameof(EF.Domicilio.IdProvincia))]
    [MapperIgnoreTarget(nameof(EF.Domicilio.IdProvinciaNavigation))]
    public partial EF.Domicilio ToEntity(Dom.Direccion source);
    public partial IEnumerable<EF.Domicilio> ToEntity(IEnumerable<Dom.Direccion> source);
    private Dom.Provincia MapToProvincia(EF.Provincia source)
            => source == null ? new Dom.Provincia() : _provinciaMapper.ToDomain(source);
}