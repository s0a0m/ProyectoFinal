using Riok.Mapperly.Abstractions;
using EF = src.Models.CodeFirst;
using Dom = src.Models.Domain;

namespace src.Models.Mappers;

[Mapper]
public partial class FilaMapper
{
    private readonly EstanteMapper _estanteMapper;

    public FilaMapper(EstanteMapper estanteMapper)
    {
        _estanteMapper = estanteMapper;
    }

    // ToDomain
    [MapperIgnoreSource(nameof(EF.Fila.IdEstante))]
    [MapperIgnoreSource(nameof(EF.Fila.UbicacionesProductos))]
    [MapperIgnoreTarget(nameof(Dom.Fila.UbicacionProducto))]
    public partial Dom.Fila ToDomain(EF.Fila source);
    public partial IEnumerable<Dom.Fila> ToDomain(IEnumerable<EF.Fila> source);

    // ToEntity
    [MapperIgnoreTarget(nameof(EF.Fila.IdEstante))]
    [MapperIgnoreTarget(nameof(EF.Fila.UbicacionesProductos))]
    [MapperIgnoreSource(nameof(Dom.Fila.UbicacionProducto))]
    public partial EF.Fila ToEntity(Dom.Fila source);
    public partial IEnumerable<EF.Fila> ToEntity(IEnumerable<Dom.Fila> source);

    private Dom.Estante MapEstante(EF.Estante source)
    {
        if (source == null) return null;
        return _estanteMapper.ToDomain(source);
    }

    private EF.Estante MapEstanteToEntity(Dom.Estante source)
    {
        if (source == null) return null;
        return _estanteMapper.ToEntity(source);
    }
}