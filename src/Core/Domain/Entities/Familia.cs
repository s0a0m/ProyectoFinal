namespace src.Models.Domain;

public class Familia
{
    public short IdFamilia { get; set; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public IEnumerable<Categoria> Categorias { get; set; } = new List<Categoria>();
}