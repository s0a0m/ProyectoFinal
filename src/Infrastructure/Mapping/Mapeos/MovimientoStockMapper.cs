using Riok.Mapperly.Abstractions;
using EF = src.Models.CodeFirst;
using Dom = src.Models.Domain;

namespace src.Models.Mappers;

[Mapper]
public partial class MovimientoStockMapper
{
    private readonly FilaMapper _filaMapper;
    private readonly ProductoMapper _productoMapper;

    public MovimientoStockMapper(FilaMapper filaMapper, ProductoMapper productoMapper)
    {
        _filaMapper = filaMapper;
        _productoMapper = productoMapper;
    }

    // ToDomain
    [MapperIgnoreSource(nameof(EF.MovimientoStock.IdProducto))]
    [MapperIgnoreSource(nameof(EF.MovimientoStock.IdFilaOrigen))]
    [MapperIgnoreSource(nameof(EF.MovimientoStock.IdFilaDestino))]
    [MapperIgnoreSource(nameof(EF.MovimientoStock.IdUsuario))]
    [MapperIgnoreSource(nameof(EF.MovimientoStock.Usuario))]
    [MapperIgnoreTarget(nameof(Dom.MovimientoStock.Usuario))]
    public partial Dom.MovimientoStock ToDomain(EF.MovimientoStock source);
    public partial IEnumerable<Dom.MovimientoStock> ToDomain(IEnumerable<EF.MovimientoStock> source);

    // ToEntity
    [MapperIgnoreTarget(nameof(EF.MovimientoStock.IdProducto))]
    [MapperIgnoreTarget(nameof(EF.MovimientoStock.IdFilaOrigen))]
    [MapperIgnoreTarget(nameof(EF.MovimientoStock.IdFilaDestino))]
    [MapperIgnoreTarget(nameof(EF.MovimientoStock.IdUsuario))]
    [MapperIgnoreTarget(nameof(EF.MovimientoStock.Usuario))]
    [MapperIgnoreSource(nameof(Dom.MovimientoStock.Usuario))]
    public partial EF.MovimientoStock ToEntity(Dom.MovimientoStock source);
    public partial IEnumerable<EF.MovimientoStock> ToEntity(IEnumerable<Dom.MovimientoStock> source);

    // ToDomain helpers
    private Dom.Producto MapProducto(EF.Producto source)
    {
        if (source == null) return null;
        return _productoMapper.ToDomain(source);
    }

    private Dom.Fila MapFilaOrigen(EF.Fila? source)
    {
        if (source == null) return new Dom.Fila();
        return _filaMapper.ToDomain(source);
    }

    private Dom.Fila MapFilaDestino(EF.Fila? source)
    {
        if (source == null) return new Dom.Fila();
        return _filaMapper.ToDomain(source);
    }

    // ToEntity helpers
    private EF.Producto MapProductoToEntity(Dom.Producto source)
    {
        if (source == null) return null;
        return _productoMapper.ToEntity(source);
    }

    private EF.Fila? MapFilaOrigenToEntity(Dom.Fila source)
    {
        if (source?.IdFila == 0) return null;
        return _filaMapper.ToEntity(source);
    }

    private EF.Fila? MapFilaDestinoToEntity(Dom.Fila source)
    {
        if (source?.IdFila == 0) return null;
        return _filaMapper.ToEntity(source);
    }
}