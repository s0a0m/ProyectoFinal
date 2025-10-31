namespace src.Models.Domain;

public class GrupoPermisos
{
    public short IdGrupoPermiso { get; init; }
    public string Nombre { get; init; }
    public string Descripcion { get; init; }
    private readonly List<Permiso> _permisos = new();
    public IReadOnlyCollection<Permiso> Permisos => _permisos.AsReadOnly();
    public bool HasPermiso(string nombrePermiso)
    {
        return _permisos.Any(p => p.Nombre.Equals(nombrePermiso, StringComparison.OrdinalIgnoreCase));
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
        _permisos.AddRange(permisos ?? Enumerable.Empty<Permiso>());
    }
}