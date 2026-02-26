using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using Dom = src.Models.Domain;
using src.Presentation.ViewModels.UsuarioVM; 

namespace src.Presentation.ViewModels.GrupoPermisoVM
{
    
    public class CrearGrupoViewModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder los 50 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(150, ErrorMessage = "La descripción no puede exceder los 150 caracteres.")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; } = string.Empty;

        public List<int> PermisosSeleccionados { get; set; } = new();

        public List<PermisoAsignadoViewModel> TodosLosPermisos { get; set; } = new();

       
        public static Dom.GrupoPermisos CargarGrupo(CrearGrupoViewModel vm)
        {
            var permisos = vm.PermisosSeleccionados
                             .Select(id => new Dom.Permiso { IdPermiso = id })
                             .ToList();
            
           
            return new Dom.GrupoPermisos(
                id: 0, 
                nombre: vm.Nombre.Trim(),
                descripcion: vm.Descripcion.Trim(),
                permisos: permisos
            );
        }
    }
}