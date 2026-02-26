// EN: Presentation/ViewModels/UsuarioVM/PermisoAsignadoViewModel.cs
namespace src.Presentation.ViewModels.UsuarioVM
{
    public class PermisoAsignadoViewModel
    {
        public int IdPermiso { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public bool Asignado { get; set; } // true si el usuario lo tiene (para el checkbox)
    }
}