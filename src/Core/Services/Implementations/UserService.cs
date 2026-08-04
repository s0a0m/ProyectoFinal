using src.Core.Services.Interfaces;
using src.Models.Domain;
using src.Repositories.Interfaces;
using src.Presentation.ViewModels.UsuarioVM;
using Dom = src.Models.Domain;
using Core.Common;

public class UserService : IUserService
{
    private readonly IUsuarioRepository _userRepository;
    private readonly IPermisoRepository _permisoRepository;
    private readonly IGrupoPermisosRepository _grupoRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public UserService(IUsuarioRepository userRepository, IPermisoRepository permisoRepository, IGrupoPermisosRepository grup, IHttpContextAccessor httpContextAccessor)
    {
        _userRepository = userRepository;
        _permisoRepository = permisoRepository;
        _grupoRepository = grup;
        _httpContextAccessor = httpContextAccessor;
    }


    //  Preparar ViewModels ---

    public Dom.Usuario? ObtenerUsuarioActual()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context == null) return null;
        short idFinal = 0;
        // Leemos el entero desde la sesión. 
        // Asumo que la key es "IdUsuario", si usas otra en tu Login, cámbiala aquí.
        var idString = context.Session.GetString("UsuarioId");
        if (!string.IsNullOrEmpty(idString) && short.TryParse(idString, out short parsedId))
        {
            idFinal = parsedId;
        }
        else
        {
            var idInt = context.Session.GetInt32("UsuarioId");
            if (idInt.HasValue)
            {
                idFinal = (short)idInt.Value;
            }
        }
        if (idFinal > 0)
        {
            return new Dom.Usuario
            {
                IdUsuario = idFinal,
                Nombre = string.Empty,
                Apellido = string.Empty,
                Correo = string.Empty,
                Telefono = string.Empty,
                Contrasenia = string.Empty,
                Identificacion = string.Empty
            };
        }

        return null;
    }



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


    public async Task<ServiceResult> CreateUserAsync(CrearUsuarioViewModel usuarioVM)
    {
        if (string.IsNullOrEmpty(usuarioVM.Contrasenia))
            return ServiceResult.Fail("La contraseña es obligatoria.");

        Dom.Usuario? existeMail = await _userRepository.ObtenerPorCorreoAsync(usuarioVM.Correo);
        if (existeMail != null)
            return ServiceResult.Fail("El correo electrónico ya se encuentra registrado.");

        Dom.Usuario? existeIdentificacion = await _userRepository.GetByIdentificationAsync(usuarioVM.Identificacion);
        if (existeIdentificacion != null)
            return ServiceResult.Fail("La identificación ya se encuentra registrada por otro usuario.");

        Dom.Usuario usuario = CrearUsuarioViewModel.CargarUsuario(usuarioVM);
        usuario.Contrasenia = BCrypt.Net.BCrypt.HashPassword(usuarioVM.Contrasenia);
        
        usuario.Activo = true;

        try
        {
            await _userRepository.AddAsync(usuario);
            return ServiceResult.Ok("Usuario creado exitosamente.");
        }
        catch (Exception)
        {
            return ServiceResult.Fail("Ocurrió un error inesperado al guardar el usuario.");
        }
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

    public async Task<ServiceResult> UpdateUserAsync(ActualizarUsuarioViewModel usuarioVM)
    {
        Dom.Usuario? usuarioExistente = await _userRepository.GetByIdAsync(usuarioVM.IdUsuario);
        if (usuarioExistente == null)
            return ServiceResult.Fail("El usuario no existe o no fue encontrado.");

        Dom.Usuario? usuarioConMismoMail = await _userRepository.ObtenerPorCorreoAsync(usuarioVM.Correo);
        if (usuarioConMismoMail != null && usuarioConMismoMail.IdUsuario != usuarioVM.IdUsuario)
            return ServiceResult.Fail("El correo electrónico ya está siendo usado por otro usuario.");

        Dom.Usuario? usuarioConMismaIdentificacion = await _userRepository.GetByIdentificationAsync(usuarioVM.Identificacion);
        if (usuarioConMismaIdentificacion != null && usuarioConMismaIdentificacion.IdUsuario != usuarioVM.IdUsuario)
            return ServiceResult.Fail("La identificación ya está registrada en otro sistema.");

        usuarioExistente.Nombre = usuarioVM.Nombre;
        usuarioExistente.Apellido = usuarioVM.Apellido;
        usuarioExistente.Identificacion = usuarioVM.Identificacion;
        usuarioExistente.Correo = usuarioVM.Correo;
        usuarioExistente.Telefono = usuarioVM.Telefono;

        usuarioExistente.PermisosUsuario = usuarioVM.PermisosSeleccionados
            .Select(id => new Dom.Permiso { IdPermiso = id })
            .ToList();

        usuarioExistente.GrupoPermisos = usuarioVM.GruposSeleccionados
            .Select(id => new Dom.GrupoPermisos { IdGrupoPermiso = (short)id })
            .ToList();

        if (!string.IsNullOrEmpty(usuarioVM.Contrasenia))
        {
            usuarioExistente.Contrasenia = BCrypt.Net.BCrypt.HashPassword(usuarioVM.Contrasenia);
        }

        try
        {
            await _userRepository.UpdateAsync(usuarioExistente);
            return ServiceResult.Ok("Usuario actualizado correctamente.");
        }
        catch (Exception)
        {
            return ServiceResult.Fail("Error interno al intentar actualizar el usuario.");
        }
    }


    public async Task<ServiceResult> DisableUserAsync(int idUsuario)
    {
        Dom.Usuario? usuario = await _userRepository.GetByIdAsync(idUsuario);

        if (usuario == null)
        {
            return ServiceResult.Fail("No se encontró el usuario que intenta desactivar.");
        }

        if (!usuario.Activo)
        {
            return ServiceResult.Fail("El usuario ya se encuentra inactivo.");
        }

        usuario.Activo = false;

        try
        {
            await _userRepository.UpdateAsync(usuario);
            return ServiceResult.Ok($"El usuario {usuario.Nombre} {usuario.Apellido} ha sido desactivado correctamente.");
        }
        catch (Exception)
        {
            return ServiceResult.Fail("Ocurrió un error inesperado al procesar la baja del usuario.");
        }
    }

    public async Task<ServiceResult> ReactivarUsuarioAsync(int idUsuario)
    {
        Dom.Usuario? usuario = await _userRepository.GetByIdAsync(idUsuario);

        if (usuario == null)
        {
            return ServiceResult.Fail("El usuario que intenta reactivar no existe en el sistema.");
        }

        if (usuario.Activo)
        {
            return ServiceResult.Fail("El usuario ya se encuentra activo.");
        }

        usuario.Activo = true;

        try
        {
            await _userRepository.UpdateAsync(usuario);
            return ServiceResult.Ok($"El usuario {usuario.Nombre} {usuario.Apellido} ha sido reactivado con éxito.");
        }
        catch (Exception)
        {
            return ServiceResult.Fail("Ocurrió un error técnico al intentar reactivar el usuario.");
        }
    }



    public async Task<Dom.Usuario?> ValidarUsuario(string correo, string clave)
    {
        var usuario = await _userRepository.ObtenerPorCorreoAsync(correo);

        if (usuario == null) return null;

        bool isValid = BCrypt.Net.BCrypt.Verify(clave, usuario.Contrasenia);
        if (!isValid) return null;
        if (usuario.Activo == false) return null;

        //LOGICA DE APLANADO DE PERMISOS
        var permisosDirectos = usuario.PermisosUsuario ?? new List<Dom.Permiso>();

        var permisosDeGrupos = usuario.GrupoPermisos != null
            ? usuario.GrupoPermisos.SelectMany(g => g.Permisos)
            : new List<Dom.Permiso>();

        var permisosUnificados = permisosDirectos
            .Concat(permisosDeGrupos)
            .DistinctBy(p => p.IdPermiso)
            .ToList();  

        usuario.PermisosUsuario = permisosUnificados;

        return usuario;
    }

}