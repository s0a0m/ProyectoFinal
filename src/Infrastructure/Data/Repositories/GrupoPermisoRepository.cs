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

        return DominioMapper.Map(efGrupos);
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
    public async Task UpdateAsync(Dom.GrupoPermisos domainEntity)
    {
        EF.GrupoPermisos grupoPermisosEF = DominioMapper.Map(domainEntity);
        _context.Update(grupoPermisosEF);
        await _context.SaveChangesAsync();
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