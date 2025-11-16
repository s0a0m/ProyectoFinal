using System.ComponentModel.DataAnnotations;
using Dom = src.Models.Domain;

namespace src.Presentation.ViewModels.UsuarioVM
{
    public class ActualizarUsuarioViewModel
    {
        [Required]
        public short IdUsuario { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(100, ErrorMessage = "El apellido no puede exceder los 100 caracteres.")]
        public string Apellido { get; set; } = string.Empty;

        [Required(ErrorMessage = "La identificación (DNI/CUIT) es obligatoria.")]
        [StringLength(11, ErrorMessage = "La identificación debe tener como máximo 11 caracteres.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "La identificación solo puede contener números.")]
        public string Identificacion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del correo no es válido.")]
        [StringLength(100, ErrorMessage = "El correo no puede exceder los 100 caracteres.")]
        public string Correo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        [StringLength(20, ErrorMessage = "El teléfono no puede exceder los 20 caracteres.")]
        [Phone(ErrorMessage = "El formato del teléfono no es válido.")]
        public string Telefono { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La nueva contraseña debe tener al menos 6 caracteres.")]
        [Display(Name = "Nueva Contraseña (Opcional)")]
        public string? Contrasenia { get; set; } // SÍ es nulable

        [DataType(DataType.Password)]
        [Compare(nameof(Contrasenia), ErrorMessage = "Las contraseñas no coinciden.")]
        [Display(Name = "Confirmar Nueva Contraseña")]
        public string? ConfirmarContrasenia { get; set; } // SÍ es nulable

        // saque la posibilidad de actualizar el estado

        // Para recibir los IDs seleccionados desde el formulario POST
        public List<int> PermisosSeleccionados { get; set; } = new();

        // Para mostrar la lista de checkboxes en el formulario GET
        public List<PermisoAsignadoViewModel> TodosLosPermisos { get; set; } = new();

        // --- FIN DE CAMBIOS ---

        public ActualizarUsuarioViewModel() { }

        public ActualizarUsuarioViewModel(Dom.Usuario u, List<Dom.Permiso> todosLosPermisos)
        {
            IdUsuario = u.IdUsuario;
            Nombre = u.Nombre;
            Apellido = u.Apellido;
            Identificacion = u.Identificacion;
            Correo = u.Correo;
            Telefono = u.Telefono;
            // Intencionalmente, NO cargue la contraseña existente en el formulario
            var permisosUsuarioIds = new HashSet<int>(u.PermisosUsuario.Select(p => p.IdPermiso));

            TodosLosPermisos = todosLosPermisos.Select(permiso => new PermisoAsignadoViewModel
            {
                IdPermiso = permiso.IdPermiso,
                Nombre = permiso.Nombre,
                Descripcion = permiso.Descripcion,
                Asignado = permisosUsuarioIds.Contains(permiso.IdPermiso) // Marcar si el usuario lo tiene
            }).ToList();
        }

        // Nota: No necesitamos un método estático 'CargarUsuario' aquí,
        // ya que la lógica de actualización en el controlador es diferente
        // (obtiene el usuario existente y actualiza sus propiedades).
    }
}