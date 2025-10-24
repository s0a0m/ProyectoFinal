using Dom = src.Models.Domain;

namespace src.Repositories.Interfaces;

public interface IUsuarioRepository
{
    Task<IEnumerable<Dom.Usuario>> GetAllUsuarioAsync();
    Task<Dom.Usuario?> GetUsuarioByIdAsync(int id);
    Task AddAsync(Dom.Usuario entity);
    Task UpdateAsync(Dom.Usuario entity);
    Task<bool> DeleteAsync(int id);
}