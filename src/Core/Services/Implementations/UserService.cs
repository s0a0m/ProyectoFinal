using src.Core.Services.Interfaces;
using src.Models.Domain;
using src.Repositories.Interfaces;
using src.ViewModels;
using Dom = src.Models.Domain;

public class UserService : IUserService
{
    private readonly IUsuarioRepository _userRepository;

    public UserService(IUsuarioRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Dom.Usuario> CreateUserAsync(CrearUsuarioViewModel usuarioVM)
    {
        if (string.IsNullOrEmpty(usuarioVM.Contrasenia))
        {
            throw new ArgumentException("La contraseña es obligatoria", nameof(usuarioVM.Contrasenia));
        }
        Dom.Usuario usuario = CrearUsuarioViewModel.CargarUsuario(usuarioVM);

        // para despues: agregar un hash de la contraseña aquí 
        usuario.Activo = true;
        await _userRepository.AddAsync(usuario);

        return usuario;
    }

    public async Task<IEnumerable<Dom.Usuario>> GetActiveUsersAsync()
    {
        var listaUsuarios = await _userRepository.GetAllAsync();
        var usuariosActivos = listaUsuarios.Where(u => u.Activo == true);
        return usuariosActivos;
    }

    public async Task<Dom.Usuario> GetUserByIdAsync(int idUser)
    {
        Dom.Usuario? usuario = await _userRepository.GetByIdAsync(idUser);

        if (usuario == null)
        {
            throw new KeyNotFoundException($"El usuario con ID {idUser} no fue encontrado.");
        }

        return usuario;
    }

    public async Task UpdateUserAsync(ActualizarUsuarioViewModel usuarioVM)
    {
        Dom.Usuario? usuarioExistente = await _userRepository.GetByIdAsync(usuarioVM.IdUsuario);

        if (usuarioExistente == null)
        {
            throw new KeyNotFoundException($"Usuario con ID {usuarioVM.IdUsuario} no encontrado.");
        }

        // Para Despues: Encapsular mapeo
        usuarioExistente.Nombre = usuarioVM.Nombre;
        usuarioExistente.Apellido = usuarioVM.Apellido;
        usuarioExistente.Identificacion = usuarioVM.Identificacion;
        usuarioExistente.Correo = usuarioVM.Correo;
        usuarioExistente.Telefono = usuarioVM.Telefono;

        if (!string.IsNullOrEmpty(usuarioVM.Contrasenia))
        {
            // Para despues: Agregar hash de la contraseña aquí
            usuarioExistente.Contrasenia = usuarioVM.Contrasenia;
        }

        await _userRepository.UpdateAsync(usuarioExistente);
    }
    public async Task DisableUserAsync(int idUsuario)
    {
        Dom.Usuario? usuario = await _userRepository.GetByIdAsync(idUsuario);

        if (usuario == null)
        {
            throw new KeyNotFoundException($"Usuario con ID {idUsuario} no encontrado.");
        }

        if (usuario.Activo == true)
        {
            usuario.Activo = false;
            await _userRepository.UpdateAsync(usuario);
        }
    }
}