using Microsoft.EntityFrameworkCore;
using EF = src.Models.CodeFirst;
using Dom = src.Models.Domain;
using src.Repositories.Interfaces;
using src.Models.Mappers;
using src.Models.CodeFirst;

namespace src.Repositories.Implementations;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly EF.AppDbContext _context;
    private readonly IPermisoRepository _repoPermiso;

    public UsuarioRepository(EF.AppDbContext context, IPermisoRepository repoPermiso)
    {
        _context = context;
        _repoPermiso = repoPermiso;
    }

    private IQueryable<EF.Usuario> GetQueryUsuario()
    {
        return _context.Usuarios
            .Include(u => u.UsuariosPermisos)
            .ThenInclude(up => up.Permiso)
            .Include(u => u.UsuariosGruposPermisos)
            .ThenInclude(ugp => ugp.GrupoPermiso);
    }

    public async Task<IEnumerable<Dom.Usuario>> GetAllAsync()
    {
        IEnumerable<EF.Usuario> usuarioEF = await GetQueryUsuario().ToListAsync();
        IEnumerable<Dom.Usuario> usuarios = DominioMapper.Map(usuarioEF);
        //  Una solución mejor sería un Include(u => u.UsuarioPermisos).ThenInclude(up => up.Permiso) 
        //  y un mapper que lo soporte, pero eso es para la deuda técnica)
        // foreach (var u in usuarios)
        // {
        //     u.Permisos = await _repoPermiso.GetPermisosByUsuarioIdAsync(u.IdUsuario);
        // }
        return usuarios;
    }

    public async Task<Dom.Usuario?> GetByIdAsync(int idUsuario)
    {
        EF.Usuario? usuarioEF = await GetQueryUsuario()
            .AsNoTracking() // <-- Añadido AsNoTracking
            .Where(u => u.IdUsuario == idUsuario)
            .FirstOrDefaultAsync();

        if (usuarioEF is null) return null;

        Dom.Usuario user = DominioMapper.Map(usuarioEF);

        // user.Permisos = await _repoPermiso.GetPermisosByUsuarioIdAsync(user.IdUsuario);
        return user;
    }


    public async Task AddAsync(Dom.Usuario entity)
    {
        EF.Usuario usuarioEF = DominioMapper.Map(entity);

        usuarioEF.IdUsuario = 0;
        await _context.Usuarios.AddAsync(usuarioEF);
        await _context.SaveChangesAsync();
        entity.IdUsuario = usuarioEF.IdUsuario;
        if (entity.PermisosUsuario != null && entity.PermisosUsuario.Any())
        {
            // await _repoPermiso.ReemplazarPermisosAsync(
            //     entity.IdUsuario,
            //     entity.Permisos.Select(p => p.IdPermiso)
            // );
            await _repoPermiso.ReemplazarPermisosAsync(
                entity.IdUsuario,
                entity.PermisosUsuario.Select(p => p.IdPermiso)
            );
        }
        // implementar luego: sirve para reemplazar los grupos de permisos de usuario
        // if (entity.GrupoPermisos != null && entity.GrupoPermisos.Any())
        // {
        //     await _repoGrupoPermiso.ReemplazarGruposAsync(
        //         entity.IdUsuario,
        //         entity.GrupoPermisos.Select(g => g.IdGrupoPermiso)
        //     );
        // }
    }

    public async Task UpdateAsync(Dom.Usuario entity)
    {
        // 1. Usar el patrón eficiente (Find + SetValues)
        var existingEntity = await _context.Usuarios.FindAsync(entity.IdUsuario);
        if (existingEntity == null)
        {
            throw new KeyNotFoundException($"Usuario con ID {entity.IdUsuario} no encontrado.");
        }

        // 2. Mapear a un temporal para copiar valores escalares
        EF.Usuario usuarioTemporalEF = DominioMapper.Map(entity);
        _context.Entry(existingEntity).CurrentValues.SetValues(usuarioTemporalEF);

        // 3. Guardar cambios escalares
        await _context.SaveChangesAsync();

        // 4. Llamar al método REEMPLAZAR para los permisos
        // (Esto maneja altas y bajas en una transacción separada pero eficiente)
        if (entity.PermisosUsuario != null)
        {
            await _repoPermiso.ReemplazarPermisosAsync(
                entity.IdUsuario,
                entity.PermisosUsuario.Select(p => p.IdPermiso)
            );
        }
    }
}