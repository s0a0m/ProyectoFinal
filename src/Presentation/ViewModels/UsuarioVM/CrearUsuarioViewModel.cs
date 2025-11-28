using System.ComponentModel.DataAnnotations;
using Dom = src.Models.Domain;
namespace src.Presentation.ViewModels.UsuarioVM
{
    /// <summary>
    /// ViewModel para el formulario de creación de nuevos usuarios.
    /// La contraseña es obligatoria aquí.
    /// </summary>
    public class CrearUsuarioViewModel
    {
        // Nota: No hay IdUsuario en el ViewModel de creación

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(50, ErrorMessage = "El apellido no puede exceder los 100 caracteres.")]
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


        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [DataType(DataType.Password)]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        public string Contrasenia { get; set; }

        [Required(ErrorMessage = "Debe confirmar la contraseña.")]
        [DataType(DataType.Password)]
        [Compare(nameof(Contrasenia), ErrorMessage = "Las contraseñas no coinciden.")]
        [Display(Name = "Confirmar Contraseña")]
        public string ConfirmarContrasenia { get; set; }

        // El estado 'Activo' por defecto al crear
        public bool Activo { get; set; } = true;

        // Para recibir los IDs seleccionados desde el formulario POST
        public List<int> PermisosSeleccionados { get; set; } = new();

        // Para mostrar la lista de checkboxes en el formulario GET
        public List<PermisoAsignadoViewModel> TodosLosPermisos { get; set; } = new();


        public static Dom.Usuario CargarUsuario(CrearUsuarioViewModel vm)
        {
            return new Dom.Usuario
            {

                Nombre = vm.Nombre.Trim(),
                Apellido = vm.Apellido.Trim(),
                Identificacion = vm.Identificacion.Trim(),
                Correo = vm.Correo.Trim(),
                Telefono = vm.Telefono.Trim(),
                Contrasenia = vm.Contrasenia,
                Activo = true,
                FechaAlta = DateTime.UtcNow,
                PermisosUsuario = vm.PermisosSeleccionados // <-- Asignamos solo los IDs
                               .Select(id => new Dom.Permiso { IdPermiso = id })
                               .ToList()
            };
        }
    }
}