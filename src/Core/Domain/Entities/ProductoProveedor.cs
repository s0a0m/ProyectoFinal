namespace src.Models.Domain;

public class ProductoProveedor
{
    public Producto Producto { get; init; }
    public Proveedor Proveedor { get; init; }
    public decimal Precio { get; set; }
    public int StockAsignado { get; set; }
    public ProductoProveedor(decimal precio, int stockAsignado)
    {
        Precio = precio;
        StockAsignado = stockAsignado;
    }
}