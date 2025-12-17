using Microsoft.EntityFrameworkCore;
using EF = src.Models.CodeFirst;
using Dom = src.Models.Domain;
using src.Repositories.Interfaces;
using src.Models.Mappers;
using src.Models.Domain;
using src.Contracts;

namespace src.Repositories.Implementations;

public class NovedadesRepository : INovedadesRepository
{
    private readonly EF.AppDbContext _context;

    public NovedadesRepository(EF.AppDbContext context)
    {
        _context = context;
    }
    private IQueryable<EF.NovedadesProveedor> GetQueryNovedadesProveedor()
    {
        return _context.NovedadesProveedores
        .Include(p => p.Proveedor);
    }
    public async Task<IEnumerable<NovedadPendiente>> GetPendientesAsync()
    {
        var novedadesef = await GetQueryNovedadesProveedor().AsNoTracking().ToListAsync();
        return DominioMapper.Map(novedadesef);
    }

    public async Task AddAsync(NovedadPendiente entity, CancellationToken cancellationToken = default)
    {
        var novedadef = DominioMapper.Map(entity);
        novedadef.FechaImportacion = DateTime.Now;
        novedadef.IdNovedad = 0;
        await _context.AddAsync(novedadef, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> ContarPendientesAsync()
    {
        return await _context.NovedadesProveedores
            .CountAsync(n => n.Estado == src.Models.Common.EstadoNovedad.PENDIENTE);
    }
}