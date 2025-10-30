namespace src.Models.Domain;

public class GrupoPermisos
{
    public short Id { get; init; }
    public string Nombre { get; init; }
    public string Descripcion { get; init; }
    private readonly List<Permiso> _permisos = new();
    public IReadOnlyCollection<Permiso> Permisos => _permisos.AsReadOnly();
    public bool HasPermiso(string nombrePermiso)
    {
        return _permisos.Any(p => p.Nombre.Equals(nombrePermiso, StringComparison.OrdinalIgnoreCase));
    }
}