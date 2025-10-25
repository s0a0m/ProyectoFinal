using src.ViewModels;
using Dom = src.Models.Domain;
namespace src.Core.Services.Interfaces;

public interface IUserService
{
    Task<Dom.Usuario> CreateUserAsync(CrearUsuarioViewModel usuarioVM);
    Task<Dom.Usuario> GetUserByIdAsync(int idUser);
    Task<IEnumerable<Dom.Usuario>> GetActiveUsersAsync();
    Task UpdateUserAsync(ActualizarUsuarioViewModel usuarioVM);
    Task DisableUserAsync(int idUsuario);
}