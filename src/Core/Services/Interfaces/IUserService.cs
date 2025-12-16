using src.Presentation.ViewModels.UsuarioVM;
using Dom = src.Models.Domain;
namespace src.Core.Services.Interfaces;

public interface IUserService
{
    Task<Dom.Usuario> CreateUserAsync(CrearUsuarioViewModel usuarioVM);
    Task<Dom.Usuario> GetUserByIdAsync(int idUser);
    Task<IEnumerable<Dom.Usuario>> GetUsersAsync();
    Task UpdateUserAsync(ActualizarUsuarioViewModel usuarioVM);
    Task DisableUserAsync(int idUsuario);
    Task<CrearUsuarioViewModel> PrepararCrearViewModelAsync();
    Task<ActualizarUsuarioViewModel> PrepararActualizarViewModelAsync(int idUsuario);
    Task<Dom.Usuario?> ValidarUsuario(string correo, string clave);
    Task RepoblarViewModelParaErrorAsync(CrearUsuarioViewModel viewModelConErrores);
    Task RepoblarViewModelParaErrorAsync(ActualizarUsuarioViewModel viewModelConErrores);
    Task ReactivarUsuarioAsync(int idUsuario);
}