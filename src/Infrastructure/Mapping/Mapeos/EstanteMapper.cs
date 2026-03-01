using Riok.Mapperly.Abstractions;
using EF = src.Models.CodeFirst;
using Dom = src.Models.Domain;

namespace src.Models.Mappers;

[Mapper]
public partial class EstanteMapper
{
    private readonly DepositoMapper _depositoMapper;

    public EstanteMapper(DepositoMapper depositoMapper)
    {
        _depositoMapper = depositoMapper;
    }

    // ToDomain
    [MapperIgnoreSource(nameof(EF.Estante.IdDeposito))]
    [MapperIgnoreSource(nameof(EF.Estante.Filas))]
    [MapperIgnoreTarget(nameof(Dom.Estante.Filas))]
    public partial Dom.Estante ToDomain(EF.Estante source);
    public partial IEnumerable<Dom.Estante> ToDomain(IEnumerable<EF.Estante> source);

    // ToEntity
    [MapperIgnoreTarget(nameof(EF.Estante.IdDeposito))]
    [MapperIgnoreTarget(nameof(EF.Estante.Filas))]
    [MapperIgnoreSource(nameof(Dom.Estante.Filas))]
    public partial EF.Estante ToEntity(Dom.Estante source);
    public partial IEnumerable<EF.Estante> ToEntity(IEnumerable<Dom.Estante> source);

    private Dom.Deposito MapDeposito(EF.Deposito source)
    {
        if (source == null) return null;
        return _depositoMapper.ToDomain(source);
    }

    private EF.Deposito MapDepositoToEntity(Dom.Deposito source)
    {
        if (source == null) return null;
        return _depositoMapper.ToEntity(source);
    }
}