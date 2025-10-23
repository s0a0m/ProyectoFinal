namespace src.Models.Domain;

public class Contado : CondicionDePago
{
    public string Tipo { get; set; }

    public Contado()
    {
        Tipo = "Contado";
    }
}