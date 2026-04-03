using Riok.Mapperly.Abstractions;
using EF = src.Models.CodeFirst;
using Dom = src.Models.Domain;

namespace src.Models.Mappers;

[Mapper]
public partial class DepositoMapper
{
    private readonly DomicilioMapper _domicilioMapper;

    public DepositoMapper(DomicilioMapper domicilioMapper)
    {
        _domicilioMapper = domicilioMapper;
    }

    // ToDomain
    [MapperIgnoreSource(nameof(EF.Deposito.IdDireccion))]
    [MapperIgnoreSource(nameof(EF.Deposito.Estantes))]
    [MapperIgnoreTarget(nameof(Dom.Deposito.Estantes))]
    [MapProperty(nameof(EF.Deposito.Direccion), nameof(Dom.Deposito.Direccion))]
    public partial Dom.Deposito ToDomain(EF.Deposito source);
    public partial IEnumerable<Dom.Deposito> ToDomain(IEnumerable<EF.Deposito> source);

    // ToEntity
    [MapperIgnoreTarget(nameof(EF.Deposito.IdDireccion))]
    [MapperIgnoreTarget(nameof(EF.Deposito.Estantes))]
    [MapperIgnoreSource(nameof(Dom.Deposito.Estantes))]
    [MapProperty(nameof(Dom.Deposito.Direccion), nameof(EF.Deposito.Direccion))]
    public partial EF.Deposito ToEntity(Dom.Deposito source);
    public partial IEnumerable<EF.Deposito> ToEntity(IEnumerable<Dom.Deposito> source);

    private Dom.Direccion MapDireccion(EF.Domicilio source)
    {
        if (source == null) return null;
        return _domicilioMapper.ToDomain(source);
    }

    private EF.Domicilio MapDireccionToEntity(Dom.Direccion source)
    {
        if (source == null) return null;
        return _domicilioMapper.ToEntity(source);
    }
}