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
        .Include(p => p.Proveedor)
        .Where(p => p.Estado == Models.Common.EstadoNovedad.PENDIENTE);
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
        novedadef.IdProducto = null;
        novedadef.FechaImportacion = DateTime.UtcNow;

        await _context.AddAsync(novedadef, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> ContarPendientesAsync()
    {
        return await _context.NovedadesProveedores
            .CountAsync(n => n.Estado == src.Models.Common.EstadoNovedad.PENDIENTE);
    }

    public async Task<NovedadPendiente?> GetByIdAsync(int id)
    {
        var efEntity = await _context.NovedadesProveedores
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdNovedad == id);
        if (efEntity is null) return null;
        return new NovedadPendiente
        {
            IdNovedad = efEntity.IdNovedad,
            IdProveedor = efEntity.IdProveedor,
            // Si agregaste IdProducto al DTO:
            IdProducto = efEntity.IdProducto ?? 0,
            CodigoBarraExterno = efEntity.CodigoBarraExterno,
            NombreSugerido = efEntity.NombreSugerido,
            PrecioSugerido = efEntity.PrecioSugerido,
            Estado = efEntity.Estado
        };
    }

    public async Task UpdateAsync(NovedadPendiente entity)
    {
        var efEntity = await _context.NovedadesProveedores.FindAsync(entity.IdNovedad);

        if (efEntity != null)
        {
            // Actualizamos solo los campos que cambian en la resolución
            efEntity.Estado = entity.Estado;
            if (entity.IdProducto > 0)
            {
                efEntity.IdProducto = entity.IdProducto;
            }
            // Si tu DTO NovedadPendiente tiene la propiedad IdProducto (debería tenerla para persistir la relación)
            // efEntity.IdProducto = entity.IdProducto; 

            // Si el DTO no tiene IdProducto pero el dominio sí, deberías mapearlo. 
            // Asumo que agregaste IdProducto a NovedadPendiente como vimos antes.

            await _context.SaveChangesAsync();
        }
    }
}