namespace src.Models.Domain;

public class Producto
{
    public int IdProducto { get; init; }
    public string Nombre { get; set; }
    public int StockMinimo { get; set; }
    public int StockTotal { get; set; }
    public bool Activo { get; set; }
    public Grupo Grupo { get; set; }
    public Categoria Categoria { get; set; }
    public CodigoBarra? CodigoBarra { get; set; }
    public IReadOnlyCollection<ProductoProveedor> RelacionesProveedor { get; init; } = new List<ProductoProveedor>();
    public Producto(int idProducto, string nombre, int stockMinimo, int stockTotal, bool activo, Grupo grupo, Categoria categoria)
    {
        if (grupo == null) throw new ArgumentNullException(nameof(grupo), "El producto debe pertenecer a un grupo.");
        if (categoria == null) throw new ArgumentNullException(nameof(categoria), "El producto debe pertenecer a una categoría.");
        IdProducto = idProducto;
        Nombre = nombre;
        StockMinimo = stockMinimo;
        StockTotal = stockTotal;
        Activo = activo;
        Grupo = grupo;
        Categoria = categoria;
    }
}