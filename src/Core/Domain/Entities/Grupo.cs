namespace src.Models.Domain;

public class Grupo
{
    public int IdGrupo { get; init; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; }

    public IReadOnlyCollection<Producto> Productos { get; init; } = new List<Producto>();

    public Grupo(int idGrupo, string nombre, string descripcion)
    {
        IdGrupo = idGrupo;
        Nombre = nombre;
        Descripcion = descripcion;
    }
}