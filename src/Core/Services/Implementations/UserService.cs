using src.Core.Services.Interfaces;
using src.Models.Domain;
using src.Repositories.Interfaces;
using src.Presentation.ViewModels.UsuarioVM;
using Dom = src.Models.Domain;

public class UserService : IUserService
{
    private readonly IUsuarioRepository _userRepository;
    private readonly IPermisoRepository _permisoRepository;
    private readonly IGrupoPermisosRepository _grupoRepository;
    public UserService(IUsuarioRepository userRepository, IPermisoRepository permisoRepository,IGrupoPermisosRepository grup)
    {
        _userRepository = userRepository;
        _permisoRepository = permisoRepository;
        _grupoRepository = grup;
    }


    // --- Nuevos Métodos para Preparar ViewModels ---

    public async Task<CrearUsuarioViewModel> PrepararCrearViewModelAsync()
    {
        var viewModel = new CrearUsuarioViewModel();
        // Llama a un método privado para cargar la lista de permisos
        await RepoblarPermisosAsync(viewModel);
        return viewModel;
    }

    public async Task<ActualizarUsuarioViewModel> PrepararActualizarViewModelAsync(int idUsuario)
    {
        Dom.Usuario? usuario = await _userRepository.GetByIdAsync(idUsuario);
        if (usuario == null)
        {
            throw new KeyNotFoundException($"Usuario con ID {idUsuario} no encontrado.");
        }
        var todosLosPermisos = (await _permisoRepository.GetAllPermisosAsync()).ToList();
        var todosLosGrupos = (await _grupoRepository.GetAllAsync()).ToList();
 
        return new ActualizarUsuarioViewModel(usuario, todosLosPermisos, todosLosGrupos);
    }

    public async Task RepoblarViewModelParaErrorAsync(CrearUsuarioViewModel viewModelConErrores)
    {
        // El VM ya tiene los datos y errores del usuario, solo le falta la lista de permisos
        await RepoblarPermisosAsync(viewModelConErrores);
    }


    public async Task RepoblarViewModelParaErrorAsync(ActualizarUsuarioViewModel viewModel)
    {
        var todosLosPermisos = await _permisoRepository.GetAllPermisosAsync();
        var todosLosGrupos = await _grupoRepository.GetAllAsync();

        var permisosSeleccionados = new HashSet<int>(viewModel.PermisosSeleccionados);
        viewModel.TodosLosPermisos = todosLosPermisos.Select(p => new PermisoAsignadoViewModel
        {
            IdPermiso = p.IdPermiso,
            Nombre = p.Nombre,
            Descripcion = p.Descripcion,
            Asignado = permisosSeleccionados.Contains(p.IdPermiso)
        }).ToList();

        var gruposSeleccionados = new HashSet<int>(viewModel.GruposSeleccionados);
        viewModel.TodosLosGrupos = todosLosGrupos.Select(g => new GrupoAsignadoViewModel
        {
            IdGrupo = g.IdGrupoPermiso,
            Nombre = g.Nombre,
            Descripcion = g.Descripcion,
            Asignado = gruposSeleccionados.Contains(g.IdGrupoPermiso),
            PermisosDelGrupo = g.Permisos.Select(p => p.Nombre).ToList()
        }).ToList();
    }
    // public async Task RepoblarViewModelParaErrorAsync(ActualizarUsuarioViewModel viewModelConErrores)
    // {
    //     // El VM ya tiene los datos y errores, solo le falta la lista de permisos
    //     var todosLosPermisos = (await _permisoRepository.GetAllPermisosAsync()).ToList();

    //     // Obtenemos los IDs que el usuario *intentó* enviar, para que no se borren del formulario
    //     var permisosUsuarioIds = new HashSet<int>(viewModelConErrores.PermisosSeleccionados);

    //     viewModelConErrores.TodosLosPermisos = todosLosPermisos.Select(p => new PermisoAsignadoViewModel
    //     {
    //         IdPermiso = p.IdPermiso,
    //         Nombre = p.Nombre,
    //         Descripcion = p.Descripcion,
    //         // Re-marca los checkboxes que el usuario había seleccionado
    //         Asignado = permisosUsuarioIds.Contains(p.IdPermiso)
    //     }).ToList();
    // }

    private async Task RepoblarPermisosAsync(CrearUsuarioViewModel viewModel)
    {
        var todosLosPermisos = await _permisoRepository.GetAllPermisosAsync();
        var todosLosGrupos = await _grupoRepository.GetAllAsync();
        viewModel.TodosLosPermisos = todosLosPermisos.Select(p => new PermisoAsignadoViewModel
        {
            IdPermiso = p.IdPermiso,
            Nombre = p.Nombre,
            Descripcion = p.Descripcion,
            // Re-marca los checkboxes que el usuario había seleccionado si falló la validación
            Asignado = viewModel.PermisosSeleccionados.Contains(p.IdPermiso)
        }).ToList();
        viewModel.TodosLosGrupos = todosLosGrupos.Select(g => new GrupoAsignadoViewModel
        {
            IdGrupo = g.IdGrupoPermiso,
            Nombre = g.Nombre,
            Descripcion = g.Descripcion,
            Asignado = viewModel.GruposSeleccionados.Contains(g.IdGrupoPermiso),
            PermisosDelGrupo = g.Permisos.Select(p => p.Nombre).ToList()
        }).ToList();
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

    public async Task<IEnumerable<Dom.Usuario>> GetUsersAsync()
    {
        var listaUsuarios = await _userRepository.GetAllAsync();
        return listaUsuarios;
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
        usuarioExistente.PermisosUsuario = usuarioVM.PermisosSeleccionados
                                       .Select(id => new Dom.Permiso { IdPermiso = id })
                                       .ToList();
        usuarioExistente.GrupoPermisos = usuarioVM.GruposSeleccionados
            .Select(id => new Dom.GrupoPermisos { IdGrupoPermiso = (short)id }).ToList();
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

    public async Task ReactivarUsuarioAsync(int idUsuario)
    {
        Dom.Usuario? usuario = await _userRepository.GetByIdAsync(idUsuario);

        if (usuario == null)
        {
            throw new KeyNotFoundException($"Usuario con ID {idUsuario} no encontrado.");
        }
        if (usuario.Activo) return;
        usuario.Activo = true;
        await _userRepository.UpdateAsync(usuario);
    }



    public async Task<Dom.Usuario?> ValidarUsuario(string correo, string clave)
    {
        var usuario = await _userRepository.ObtenerPorCorreoAsync(correo);

        if (usuario == null) return null;

       
        //Cambiar a BCrypt o Hashing seguro en el futuro
        if (usuario.Contrasenia != clave) return null;

        //LOGICA DE APLANADO DE PERMISOS
        var permisosDirectos = usuario.PermisosUsuario ?? new List<Dom.Permiso>();

        var permisosDeGrupos = usuario.GrupoPermisos != null 
            ? usuario.GrupoPermisos.SelectMany(g => g.Permisos) 
            : new List<Dom.Permiso>();

        //Unir y Eliminar Duplicados
        var permisosUnificados = permisosDirectos
            .Concat(permisosDeGrupos)
            .DistinctBy(p => p.IdPermiso) 
            .ToList();

        // 4. Asignamos la lista limpia al usuario para que el Controlador la use fácil
        usuario.PermisosUsuario = permisosUnificados;

        return usuario;
    }

}