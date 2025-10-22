using Microsoft.EntityFrameworkCore;
using EF = src.Models.CodeFirst;
using Dom = src.Models.Domain;
using src.Repositories.Interfaces;
using src.Models.Mappers;

namespace src.Repositories.Implementations;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly EF.AppDbContext _context;

    public UsuarioRepository(EF.AppDbContext context)
    {
        _context = context;
    }

    private IQueryable<EF.Usuario> GetQueryUsuario()
    {
        return _context.Usuarios;
    }

    public async Task<IEnumerable<Dom.Usuario>> GetAllUsuarioAsync()
    {
        IEnumerable<EF.Usuario> usuarioEF = await GetQueryUsuario().ToListAsync();
        IEnumerable<Dom.Usuario> usuarios = DominioMapper.Map(usuarioEF);
        return usuarios;
    }

    public async Task<Dom.Usuario?> GetUsuarioByIdAsync(int idUsuario)
    {
        EF.Usuario? usuarioEF = await GetQueryUsuario()
            .Where(u => u.IdUsuario == idUsuario)
            .FirstOrDefaultAsync();

        if (usuarioEF is null) return null;

        Dom.Usuario usuarioDOM = DominioMapper.Map(usuarioEF);
        return usuarioDOM;
    }

    public async Task AddAsync(Dom.Usuario entity)
    {
        EF.Usuario usuarioEF = DominioMapper.Map(entity);
        await _context.Usuarios.AddAsync(usuarioEF);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Dom.Usuario entity)
    {
        EF.Usuario usuarioEF = DominioMapper.Map(entity);
        _context.Usuarios.Update(usuarioEF);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        // Asumiendo soft-delete (Baja lógica)
        var usuarioEF = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.IdUsuario == id);

        if (usuarioEF == null)
            return false;

        // Asumiendo que EF.Usuario tiene una propiedad 'Activo'
        usuarioEF.Activo = false;
        await _context.SaveChangesAsync();
        return true;
    }
}