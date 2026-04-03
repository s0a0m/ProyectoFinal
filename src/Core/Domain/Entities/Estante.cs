namespace src.Models.Domain;
public class Estante{
    public int IdEstante { get; set; }
    public string NumeroEstante { get; set; }=string.Empty;
    public Deposito Deposito { get; set; } = new Deposito();
    public bool Activo { get; set; }
    public bool TieneEspacio { get; set;}
    public string? Observaciones { get; set; }
    public IEnumerable<Fila> Filas { get; set; } = new List<Fila>();
}