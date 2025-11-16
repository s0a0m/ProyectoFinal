namespace src.Models.Domain;

public class GrupoPermisos
{
    public short IdGrupoPermiso { get; init; }
    public string Nombre { get; init; }
    public string Descripcion { get; init; }
    public ICollection<Permiso> Permisos { get; private set; } = new List<Permiso>();
    public bool HasPermiso(string nombrePermiso)
    {
        return Permisos.Any(p => p.Nombre.Equals(nombrePermiso, StringComparison.OrdinalIgnoreCase));
    }
    public GrupoPermisos() { }
    public GrupoPermisos(
        short id,
        string nombre,
        string descripcion,
        IEnumerable<Permiso> permisos)
    {

        IdGrupoPermiso = id;
        Nombre = nombre;
        Descripcion = descripcion;
        Permisos = permisos?.ToList() ?? new List<Permiso>();
    }
}