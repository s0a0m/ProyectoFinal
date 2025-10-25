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

    public async Task<IEnumerable<Dom.Usuario>> GetAllAsync()
    {
        IEnumerable<EF.Usuario> usuarioEF = await GetQueryUsuario().ToListAsync();
        IEnumerable<Dom.Usuario> usuarios = DominioMapper.Map(usuarioEF);
        return usuarios;
    }

    public async Task<Dom.Usuario?> GetByIdAsync(int idUsuario)
    {
        EF.Usuario? usuarioEF = await GetQueryUsuario()
            .Where(u => u.IdUsuario == idUsuario)
            .FirstOrDefaultAsync();

        return usuarioEF is null ? null : DominioMapper.Map(usuarioEF);
    }

    public async Task AddAsync(Dom.Usuario entity)
    {
        EF.Usuario usuarioEF = DominioMapper.Map(entity);
        usuarioEF.IdUsuario = 0;
        await _context.Usuarios.AddAsync(usuarioEF);
        await _context.SaveChangesAsync();
        entity.IdUsuario = usuarioEF.IdUsuario;
    }

    public async Task UpdateAsync(Dom.Usuario entity)
    {
        EF.Usuario usuarioEF = DominioMapper.Map(entity);
        _context.Usuarios.Update(usuarioEF);
        await _context.SaveChangesAsync();
    }
}