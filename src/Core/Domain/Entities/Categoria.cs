namespace src.Models.Domain;

public class Categoria
{
    public short IdCategoria { get; init; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public Familia Familia { get; set; }
    public Categoria() { }
}