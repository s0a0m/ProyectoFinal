using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using src.Repositories.Interfaces;
using Dom = src.Models.Domain;
using EF = src.Models.CodeFirst;
using src.Models.Mappers;
using src.Models.Domain; // Necesario para usar DominioMapper

namespace src.Repositories.Implementations;

public class GrupoPermisosRepository : IGrupoPermisosRepository
{
    private readonly EF.AppDbContext _context;

    public GrupoPermisosRepository(EF.AppDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<Dom.GrupoPermisos>> GetAllAsync()
    {
        var efGrupos = await _context.GruposPermisos
            .Include(g => g.GruposPermisosPermisos)
            .ThenInclude(gpp => gpp.Permiso)
            .AsNoTracking()
            .ToListAsync();

        // Mapear manualmente para evitar el mapper roto (igual que en GetByIdAsync)
        return efGrupos.Select(efGrupo => new Dom.GrupoPermisos(
            efGrupo.IdGrupoPermiso,
            efGrupo.Nombre,
            efGrupo.Descripcion,
            efGrupo.GruposPermisosPermisos
                .Where(gpp => gpp.Permiso != null)
                .Select(gpp => DominioMapper.Map(gpp.Permiso))
                .ToList()
        )).ToList();
    }

    public async Task<Dom.GrupoPermisos?> GetByIdAsync(short id)
    {
        var efGrupo = await _context.GruposPermisos
            .Include(g => g.GruposPermisosPermisos)
            .ThenInclude(gpp => gpp.Permiso)
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.IdGrupoPermiso == id);

        return efGrupo == null ? null : new Dom.GrupoPermisos(
                efGrupo.IdGrupoPermiso,
                efGrupo.Nombre,
                efGrupo.Descripcion,
                efGrupo.GruposPermisosPermisos
                    .Where(gpp => gpp.Permiso != null)
                    .Select(gpp => DominioMapper.Map(gpp.Permiso))
                    .ToList()
            );
    }

    public async Task<bool> AddPermisoToGrupo(short idGrupo, int idPermiso)
    {
        var exists = await _context.GruposPermisosPermisos
            .AnyAsync(gpp => gpp.IdGrupoPermiso == idGrupo && gpp.IdPermiso == idPermiso);

        if (exists) return true;

        var unionEntity = new EF.GrupoPermisoPermiso
        {
            IdGrupoPermiso = idGrupo,
            IdPermiso = idPermiso
        };

        _context.GruposPermisosPermisos.Add(unionEntity);
        return await _context.SaveChangesAsync() >= 1;
    }

    public async Task<Dom.GrupoPermisos> AddAsync(Dom.GrupoPermisos domainEntity)
    {
        // 1. Mapear y guardar la entidad principal (sin relaciones N-N)
        var efGrupo = DominioMapper.Map(domainEntity);
        efGrupo.IdGrupoPermiso = 0; // Asegurar que es nuevo

        _context.GruposPermisos.Add(efGrupo);
        await _context.SaveChangesAsync();

        // 2. Asignar los permisos (si los hay)
        if (domainEntity.Permisos.Any())
        {
            await ReemplazarPermisosAsync(efGrupo.IdGrupoPermiso, domainEntity.Permisos.Select(p => p.IdPermiso));
        }

        // Devolver la entidad de dominio actualizada (con el ID)
        domainEntity = new Dom.GrupoPermisos(
            efGrupo.IdGrupoPermiso,
            domainEntity.Nombre,
            domainEntity.Descripcion,
            domainEntity.Permisos
        );
        return domainEntity;
    }
    
    public async Task UpdateAsync(Dom.GrupoPermisos domainEntity)
    {
        // Este método ahora solo actualiza propiedades escalares
        var existingEntity = await _context.GruposPermisos.FindAsync(domainEntity.IdGrupoPermiso);
        if (existingEntity == null)
        {
            throw new KeyNotFoundException($"GrupoPermiso con ID {domainEntity.IdGrupoPermiso} no encontrado.");
        }

        // Mapear a temporal para copiar valores
        var temporalEF = DominioMapper.Map(domainEntity);
        _context.Entry(existingEntity).CurrentValues.SetValues(temporalEF);
        
        await _context.SaveChangesAsync();
    }
    
    public async Task ReemplazarPermisosAsync(short idGrupo, IEnumerable<int> nuevosIdsPermisos)
    {
        var relacionesActuales = await _context.GruposPermisosPermisos
            .Where(gpp => gpp.IdGrupoPermiso == idGrupo)
            .ToListAsync();
        
        var idsActuales = relacionesActuales.Select(gpp => gpp.IdPermiso).ToHashSet();
        var idsNuevos = nuevosIdsPermisos.ToHashSet();

        var relacionesParaQuitar = relacionesActuales
            .Where(gpp => !idsNuevos.Contains(gpp.IdPermiso))
            .ToList();

        var idsParaAgregar = idsNuevos
            .Where(id => !idsActuales.Contains(id))
            .ToList();

        var relacionesParaAgregar = idsParaAgregar.Select(idPermiso => new EF.GrupoPermisoPermiso
        {
            IdGrupoPermiso = idGrupo,
            IdPermiso = idPermiso
        }).ToList();

        if (relacionesParaQuitar.Any())
        {
            _context.GruposPermisosPermisos.RemoveRange(relacionesParaQuitar);
        }
        if (relacionesParaAgregar.Any())
        {
            _context.GruposPermisosPermisos.AddRange(relacionesParaAgregar);
        }

        if (relacionesParaQuitar.Any() || relacionesParaAgregar.Any())
        {
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> RemovePermisoFromGrupo(short idGrupo, int idPermiso)
    {
        var unionEntity = await _context.GruposPermisosPermisos
            .FirstOrDefaultAsync(gpp => gpp.IdGrupoPermiso == idGrupo && gpp.IdPermiso == idPermiso);

        if (unionEntity == null)
        {
            return true;
        }

        _context.GruposPermisosPermisos.Remove(unionEntity);
        return await _context.SaveChangesAsync() >= 1;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        short shortId = (short)id;

        var grupoToDelete = await _context.GruposPermisos
            .FirstOrDefaultAsync(g => g.IdGrupoPermiso == shortId);

        if (grupoToDelete == null) return false;

        _context.GruposPermisos.Remove(grupoToDelete);

        return await _context.SaveChangesAsync() > 0;
    }
}