using Microsoft.EntityFrameworkCore;
using EF = src.Models.CodeFirst;
using Dom = src.Models.Domain;
using src.Repositories.Interfaces;
using src.Models.Mappers;
using src.Models.Domain;

namespace src.Repositories.Implementations;

public class PermisoRepository : IPermisoRepository
{
    private readonly EF.AppDbContext _context;

    public PermisoRepository(EF.AppDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<Dom.Permiso>> GetAllPermisosAsync()
    {
        var efPermisos = await _context.Permisos.AsNoTracking().ToListAsync();
        return DominioMapper.Map(efPermisos);
    }
    public async Task<Dom.Permiso?> GetPermisoById(int id)
    {
        var efPermiso = await _context.Permisos.AsNoTracking().FirstOrDefaultAsync(p => p.IdPermiso == id);
        if (efPermiso == null)
        {
            return null;
        }
        return DominioMapper.Map(efPermiso);
    }
    public async Task<bool> AsignarPermisoAUsuario(int idPermiso, int idUsuario)
    {
        var efUsuario = await _context.Usuarios.FindAsync((short)idUsuario);
        if (efUsuario == null)
        {
            return false;
        }
        var existe = await _context.UsuariosPermisos.AnyAsync(up => up.IdUsuario == idUsuario && up.IdPermiso == idPermiso);

        if (existe)
        {
            return true;
        }

        var nuevaRelacion = new EF.UsuarioPermiso
        {
            IdUsuario = (short)idUsuario,
            IdPermiso = idPermiso
        };
        _context.UsuariosPermisos.Add(nuevaRelacion);

        try
        {
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException)
        {
            // Manejar errores de FK si el usuario o permiso no existen
            // Aunque se podrían verificar antes, SaveChanges es más eficiente para validarlo
            return false;
        }
    }

    public async Task<bool> AsignarPermisosAUsuario(IEnumerable<int> idsPermisos, int idUsuario)
    {
        var efUsuario = await _context.Usuarios.FindAsync((short)idUsuario);
        if (efUsuario == null)
        {
            return false;
        }
        if (idsPermisos == null || !idsPermisos.Any())
        {
            return true;
        }

        // 1. Obtener las relaciones ya existentes para el usuario
        var relacionesExistentes = await _context.UsuariosPermisos
            .Where(up => up.IdUsuario == idUsuario)
            .Select(up => up.IdPermiso)
            .ToListAsync();

        // 2. Filtrar IDs de permisos que NO están asignados aún

        // filtrar permisos que no existen en la tabla Permisos
        var permisosExistentes = await _context.Permisos
            .Where(p => idsPermisos.Contains(p.IdPermiso))
            .Select(p => p.IdPermiso)
            .ToListAsync();
        idsPermisos = permisosExistentes;
        var idsParaAgregar = idsPermisos.Except(relacionesExistentes).ToList();

        if (!idsParaAgregar.Any())
        {
            return true;
        }


        // 3. Crear las nuevas entidades de unión
        var nuevasRelaciones = idsParaAgregar.Select(idPermiso => new EF.UsuarioPermiso
        {
            IdUsuario = (short)idUsuario,
            IdPermiso = idPermiso
        }).ToList();

        // 4. Agregar el rango y guardar
        _context.UsuariosPermisos.AddRange(nuevasRelaciones);

        try
        {
            // Se realiza un único SaveChanges para todas las inserciones
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException)
        {
            // Error al insertar (e.g., ID de usuario o permiso inexistente)
            return false;
        }
    }

    // Asegúrate de que este código se agregue a la clase PermisoRepository que ya tienes

    public async Task<IEnumerable<Permiso>> GetPermisosByUsuarioIdAsync(int idUsuario)
    {
        var efPermisos = await _context.UsuariosPermisos
            .AsNoTracking()
            .Where(up => up.IdUsuario == idUsuario)
            .Select(up => up.Permiso)
            .Where(p => p != null)
            .Cast<EF.Permiso>() // <-- Le decimos al compilador que ya no hay nulls
            .ToListAsync();

        if (efPermisos.Count == 0)
        {
            return Enumerable.Empty<Permiso>();
        }

        return DominioMapper.Map(efPermisos);
    }

}