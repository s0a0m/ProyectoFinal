using Riok.Mapperly.Abstractions;
using EF = src.Models.CodeFirst;
using Dom = src.Models.Domain;

namespace src.Models.Mappers;

[Mapper]
public partial class UbicacionProductoMapper
{
    private readonly FilaMapper _filaMapper;

    public UbicacionProductoMapper(FilaMapper filaMapper)
    {
        _filaMapper = filaMapper;
    }

    // ToDomain sin Producto para evitar circular desde ProductoMapper
    [MapperIgnoreSource(nameof(EF.UbicacionProducto.IdProducto))]
    [MapperIgnoreSource(nameof(EF.UbicacionProducto.IdFila))]
    [MapperIgnoreSource(nameof(EF.UbicacionProducto.Producto))]
    [MapperIgnoreTarget(nameof(Dom.UbicacionProducto.Producto))]
    public partial Dom.UbicacionProducto ToDomainSinProducto(EF.UbicacionProducto source);

    [MapperIgnoreSource(nameof(EF.UbicacionProducto.IdProducto))]
    [MapperIgnoreSource(nameof(EF.UbicacionProducto.IdFila))]
    public partial Dom.UbicacionProducto ToDomain(EF.UbicacionProducto source);

    // ToEntity
    [MapperIgnoreTarget(nameof(EF.UbicacionProducto.IdProducto))]
    [MapperIgnoreTarget(nameof(EF.UbicacionProducto.IdFila))]
    [MapperIgnoreTarget(nameof(EF.UbicacionProducto.Producto))]
    [MapperIgnoreSource(nameof(Dom.UbicacionProducto.Producto))]
    public partial EF.UbicacionProducto ToEntity(Dom.UbicacionProducto source);
    public partial IEnumerable<EF.UbicacionProducto> ToEntity(IEnumerable<Dom.UbicacionProducto> source);

    private Dom.Fila MapFila(EF.Fila source)
    {
        if (source == null) return null;
        return _filaMapper.ToDomain(source);
    }

    private EF.Fila MapFilaToEntity(Dom.Fila source)
    {
        if (source == null) return null;
        return _filaMapper.ToEntity(source);
    }
}