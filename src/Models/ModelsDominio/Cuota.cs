namespace src.Models.Domain;

public class Cuota : CondicionDePago
{
    public short Cuotas { get; set; }
    public decimal InteresPorcentual { get; set; }
}