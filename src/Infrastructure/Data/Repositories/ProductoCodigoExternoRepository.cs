using Microsoft.EntityFrameworkCore;
using EF = src.Models.CodeFirst;
using Dom = src.Models.Domain;
using src.Repositories.Interfaces;
using src.Models.Mappers;
using src.Models.Domain;

namespace src.Repositories.Implementations;

public class ProductoCodigoExternoRepository : IProductoCodigoExternoRepository
{
    private readonly EF.AppDbContext _context;

    public ProductoCodigoExternoRepository(EF.AppDbContext context)
    {
        _context = context;
    }

    private IQueryable<EF.ProductoCodigoExterno> GetQueryProductoCodigosExternos()
    {
        return _context.ProductoCodigosExternos
        .Include(p => p.Producto);
    }

    public async Task<Producto?> ObtenerProductoPorCodigoAsync(string codigoExterno, short idProveedor, CancellationToken cancellationToken = default)
    {
        var cb = await GetQueryProductoCodigosExternos()
            .Where(p => p.CodigoBarraProveedor == codigoExterno && p.IdProveedor == idProveedor)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        return cb is null ? null : DominioMapper.Map(cb.Producto);
    }

    public async Task<bool> ExistsAsync(string codigo, short id_proveedor)
    {
        if (string.IsNullOrWhiteSpace(codigo)) return false;
        return await _context.ProductoCodigosExternos
            .AnyAsync(x => x.CodigoBarraProveedor == codigo.Trim() && x.IdProveedor == id_proveedor);
    }

    public async Task AddAsync(short idProducto, short idProveedor, string codigo)
    {
        // Mapeamos directamente a la entidad de EF CodeFirst
        var efEntity = new EF.ProductoCodigoExterno
        {
            IdProducto = idProducto,
            IdProveedor = idProveedor,
            CodigoBarraProveedor = codigo.Trim()
        };

        await _context.ProductoCodigosExternos.AddAsync(efEntity);
        await _context.SaveChangesAsync();
    }

    public async Task<IDictionary<string, EF.ProductoProveedor>> ObtenerDiccionarioPorCodigosAsync(
        List<string> codigos,
        short idProveedor,
        CancellationToken ct)
    {
        if (codigos == null || !codigos.Any())
            return new Dictionary<string, EF.ProductoProveedor>();

        var query = from pce in _context.ProductoCodigosExternos
                    join pp in _context.ProductosProveedores
                         on pce.IdProducto equals pp.IdProducto
                    where pce.IdProveedor == idProveedor
                          && pp.IdProveedor == idProveedor
                          && codigos.Contains(pce.CodigoBarraProveedor)
                    select new
                    {
                        Codigo = pce.CodigoBarraProveedor,
                        EntidadPP = pp
                    };

        var resultado = await query.ToDictionaryAsync(
            item => item.Codigo,
            item => item.EntidadPP,
            cancellationToken: ct
        );

        return resultado;
    }
}