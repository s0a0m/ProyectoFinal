using Dom = src.Models.Domain;

namespace src.Repositories.Interfaces;

public interface IUsuarioRepository
{
    Task<IEnumerable<Dom.Usuario>> GetAllAsync();
    Task<Dom.Usuario?> GetByIdAsync(int id);
    Task AddAsync(Dom.Usuario entity);
    Task UpdateAsync(Dom.Usuario entity);
}