namespace src.Models.Domain;

public class Producto
{
    public int IdProducto { get; set; }
    public string Nombre { get; set; }
    public int StockMinimo { get; set; }
    public int StockTotal { get; set; }
    public bool Activo { get; set; }
    // public IEnumerable<Grupo> Grupo { get; set; } = new List<Grupo>();
    public IEnumerable<Categoria> Categoria { get; set; } = new List<Categoria>();
    public IEnumerable<CodigoBarra> CodigoBarra { get; set; }
    public IEnumerable<UbicacionProducto> UbicacionProducto { get; set; } = new List<UbicacionProducto>();
}