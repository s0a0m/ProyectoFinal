namespace src.Models.Domain;

public class Categoria
{
    public int IdCategoria { get; init; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public IReadOnlyCollection<Producto> Productos { get; init; } = new List<Producto>();
    public Categoria(int idCategoria, string nombre, string descripcion)
    {
        IdCategoria = idCategoria;
        Nombre = nombre;
        Descripcion = descripcion;
    }
}