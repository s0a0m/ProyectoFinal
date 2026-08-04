namespace src.Models.Domain;
public class Fila
{
    public int IdFila { get; set; }
    public string NFila { get; set; }=string.Empty;
    public Estante Estante { get; set; } = new Estante();
    public bool Activo { get; set; }
    public bool TieneEspacio { get; set;}
    public string? Observaciones { get; set; }
    public IEnumerable<UbicacionProducto> UbicacionProducto { get; set; } = new List<UbicacionProducto>();
}