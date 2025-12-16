namespace src.Presentation.ViewModels.UsuarioVM
{
    public class GrupoAsignadoViewModel
    {
        public short IdGrupo { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public bool Asignado { get; set; }
        public List<string> PermisosDelGrupo { get; set; } = new();
    }
}