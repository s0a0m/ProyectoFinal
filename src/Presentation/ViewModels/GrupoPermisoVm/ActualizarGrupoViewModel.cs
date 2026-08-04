using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using Dom = src.Models.Domain;
using src.Presentation.ViewModels.UsuarioVM; // Para reusar PermisoAsignadoViewModel

namespace src.Presentation.ViewModels.GrupoPermisoVM
{
    /// <summary>
    /// ViewModel para el formulario de actualización de Grupos de Permisos.
    /// </summary>
    public class ActualizarGrupoViewModel
    {
        [Required]
        public short IdGrupoPermiso { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder los 50 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(150, ErrorMessage = "La descripción no puede exceder los 150 caracteres.")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; } = string.Empty;

        // --- Manejo de Permisos (Igual que en Usuario) ---

        // Para recibir los IDs seleccionados desde el formulario POST
        public List<int> PermisosSeleccionados { get; set; } = new();

        // Para mostrar la lista de checkboxes en el formulario GET
        public List<PermisoAsignadoViewModel> TodosLosPermisos { get; set; } = new();


        /// <summary>
        /// Constructor vacío necesario para el Model Binding en POST.
        /// </summary>
        public ActualizarGrupoViewModel() { }

        /// <summary>
        /// Constructor para rellenar el ViewModel desde el modelo de Dominio.
        /// Usado en el controlador [HttpGet] ActualizarGrupo.
        /// </summary>
        public ActualizarGrupoViewModel(Dom.GrupoPermisos g, List<Dom.Permiso> todosLosPermisos)
        {
            IdGrupoPermiso = g.IdGrupoPermiso;
            Nombre = g.Nombre;
            Descripcion = g.Descripcion;

            // --- Lógica para rellenar los checkboxes ---
            // (Idéntica a la de ActualizarUsuarioViewModel)
            
            var permisosAsignadosIds = new HashSet<int>(g.Permisos.Select(p => p.IdPermiso));
            
            TodosLosPermisos = todosLosPermisos.Select(permiso => new PermisoAsignadoViewModel
            {
                IdPermiso = permiso.IdPermiso,
                Nombre = permiso.Nombre,
                Descripcion = permiso.Descripcion,
                Asignado = permisosAsignadosIds.Contains(permiso.IdPermiso) // Marcar si el grupo lo tiene
            }).ToList();
        }
    }
}