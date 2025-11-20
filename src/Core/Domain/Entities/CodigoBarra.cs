namespace src.Models.Domain;

public class CodigoBarra
{
    public int IdCodigoBarra { get; set; }
    public string Codigo { get; set; }
    public CodigoBarra(int idCodigoBarra, string codigo)
    {
        IdCodigoBarra = idCodigoBarra;
        Codigo = codigo;
    }
    public CodigoBarra() { }
}