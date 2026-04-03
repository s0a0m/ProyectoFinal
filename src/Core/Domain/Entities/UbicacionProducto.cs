namespace src.Models.Domain;

public class UbicacionProducto 
{
    public Producto Producto { get; set; } = new Producto();
    public Fila Fila { get; set; } = new Fila();
    public decimal Cantidad { get; set; } 
}