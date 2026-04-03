namespace src.Models.Domain;
public class Deposito
{
    public int IdDeposito { get; set; }
    public string Nombre { get; set; }=string.Empty;
    public Direccion Direccion { get; set; } = new Direccion();
    public bool Activo { get; set; }
    public IEnumerable<Estante> Estantes { get; set; } = new List<Estante>();
}