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
    private readonly IGrupoPermisosRepository _repoGrupoPermiso;

    public UsuarioRepository(EF.AppDbContext context, IPermisoRepository repoPermiso, IGrupoPermisosRepository repoGrupo)
    {
        _context = context;
        _repoPermiso = repoPermiso;
        _repoGrupoPermiso = repoGrupo;
    }

    private IQueryable<EF.Usuario> GetQueryUsuario()
    {
        return _context.Usuarios
            .Include(u => u.UsuariosPermisos)
                .ThenInclude(up => up.Permiso)
            .Include(u => u.UsuariosGruposPermisos)
                .ThenInclude(ugp => ugp.GrupoPermiso)
                    .ThenInclude(gp => gp.GruposPermisosPermisos)
                        .ThenInclude(gpp => gpp.Permiso);
    }

    public async Task<IEnumerable<Dom.Usuario>> GetAllAsync()
    {
        IEnumerable<EF.Usuario> usuarioEF = await GetQueryUsuario().AsNoTracking().ToListAsync();
        IEnumerable<Dom.Usuario> usuarios = DominioMapper.Map(usuarioEF);
        return usuarios;
    }

    public async Task<Dom.Usuario?> GetByIdAsync(int idUsuario)
    {
        EF.Usuario? usuarioEF = await GetQueryUsuario()
            .AsNoTracking()
            .Where(u => u.IdUsuario == idUsuario)
            .FirstOrDefaultAsync();
        if (usuarioEF is null) return null;
        Dom.Usuario user = DominioMapper.Map(usuarioEF);
        return user;
    }


    public async Task AddAsync(Dom.Usuario entity)
    {
        EF.Usuario usuarioEF = DominioMapper.Map(entity);
        usuarioEF.IdUsuario = 0;
        // Limpiamos las navegaciones para evitar duplicados si EF intenta insertar hijos
        // Las relaciones se manejan manualmente abajo.
        usuarioEF.UsuariosPermisos.Clear();
        usuarioEF.UsuariosGruposPermisos.Clear();

        await _context.Usuarios.AddAsync(usuarioEF);
        await _context.SaveChangesAsync();
        entity.IdUsuario = usuarioEF.IdUsuario;

        if (entity.PermisosUsuario != null && entity.PermisosUsuario.Any())
        {
            await _repoPermiso.ReemplazarPermisosAsync(
                entity.IdUsuario,
                entity.PermisosUsuario.Select(p => p.IdPermiso)
            );
        }

        if (entity.GrupoPermisos != null && entity.GrupoPermisos.Any())
        {
            await ReemplazarGruposUsuarioAsync(
                entity.IdUsuario,
                entity.GrupoPermisos.Select(g => g.IdGrupoPermiso)
            );
        }
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

        if (entity.GrupoPermisos != null)
        {
            await ReemplazarGruposUsuarioAsync(
                entity.IdUsuario,
                entity.GrupoPermisos.Select(g => g.IdGrupoPermiso)
            );
        }
    }

    public async Task<Dom.Usuario?> ObtenerPorCorreoAsync(string correo)
    {
        EF.Usuario? usuarioEF = await GetQueryUsuario()
            .AsNoTracking()
            .Where(u => u.Correo == correo)
            .FirstOrDefaultAsync();

        if (usuarioEF is null) return null;
        return DominioMapper.Map(usuarioEF);
    }

    public async Task<Dom.Usuario?> GetByIdentificationAsync(string identification)
    {
        EF.Usuario? usuarioEF = await GetQueryUsuario()
            .AsNoTracking()
            .Where(u => u.Identificacion == identification)
            .FirstOrDefaultAsync();

        if (usuarioEF is null) return null;
        return DominioMapper.Map(usuarioEF);
    }


    private async Task ReemplazarGruposUsuarioAsync(int idUsuario, IEnumerable<short> nuevosIdsGrupos)
    {
        // A. Obtener asignaciones actuales
        var relacionesActuales = await _context.UsuariosGruposPermisos
            .Where(ugp => ugp.IdUsuario == idUsuario)
            .ToListAsync();

        var idsActuales = relacionesActuales.Select(ugp => ugp.IdGrupoPermiso).ToHashSet();
        var idsNuevos = nuevosIdsGrupos.ToHashSet();

        // B. Calcular qué quitar (estaba antes, pero no en la nueva lista)
        var relacionesParaQuitar = relacionesActuales
            .Where(ugp => !idsNuevos.Contains(ugp.IdGrupoPermiso))
            .ToList();

        // C. Calcular qué agregar (está en la nueva lista, pero no estaba antes)
        var idsParaAgregar = idsNuevos
            .Where(id => !idsActuales.Contains(id))
            .ToList();

        var relacionesParaAgregar = idsParaAgregar.Select(idGrupo => new EF.UsuarioGrupoPermisos
        {
            IdUsuario = (short)idUsuario,
            IdGrupoPermiso = idGrupo
        }).ToList();

        // D. Ejecutar cambios en BD
        if (relacionesParaQuitar.Any())
        {
            _context.UsuariosGruposPermisos.RemoveRange(relacionesParaQuitar);
        }

        if (relacionesParaAgregar.Any())
        {
            _context.UsuariosGruposPermisos.AddRange(relacionesParaAgregar);
        }

        if (relacionesParaQuitar.Any() || relacionesParaAgregar.Any())
        {
            await _context.SaveChangesAsync();
        }
    }

}