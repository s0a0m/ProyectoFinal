using src.Presentation.ViewModels.UsuarioVM;
using Dom = src.Models.Domain;
namespace src.Core.Services.Interfaces;

public interface IUserService
{
    Task<Dom.Usuario> CreateUserAsync(CrearUsuarioViewModel usuarioVM);
    Task<Dom.Usuario> GetUserByIdAsync(int idUser);
    Task<IEnumerable<Dom.Usuario>> GetActiveUsersAsync();
    Task UpdateUserAsync(ActualizarUsuarioViewModel usuarioVM);
    Task DisableUserAsync(int idUsuario);
    Task<CrearUsuarioViewModel> PrepararCrearViewModelAsync();
    Task<ActualizarUsuarioViewModel> PrepararActualizarViewModelAsync(int idUsuario);

    Task RepoblarViewModelParaErrorAsync(CrearUsuarioViewModel viewModelConErrores);
    Task RepoblarViewModelParaErrorAsync(ActualizarUsuarioViewModel viewModelConErrores);
}