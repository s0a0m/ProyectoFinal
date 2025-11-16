namespace src.Models.Domain;

public class Categoria
{
    public int IdCategoria { get; init; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public Categoria(int idCategoria, string nombre, string descripcion)
    {
        IdCategoria = idCategoria;
        Nombre = nombre;
        Descripcion = descripcion;
    }
    public Categoria() { }
}