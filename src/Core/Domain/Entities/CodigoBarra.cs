namespace src.Models.Domain;

public class CodigoBarra
{
    public int IdCodigoBarra { get; init; }
    public string Codigo { get; set; }

    public Producto Producto { get; init; }
    public CodigoBarra(int idCodigoBarra, string codigo, Producto producto)
    {
        IdCodigoBarra = idCodigoBarra;
        Codigo = codigo;
        Producto = producto;
    }
}