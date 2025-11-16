namespace src.Models.Domain;

public class ProductoProveedor
{
    public Producto Producto { get; set; }
    public Proveedor Proveedor { get; set; }
    public decimal Precio { get; set; }
    public int StockAsignado { get; set; }
    public ProductoProveedor(decimal precio, int stockAsignado)
    {
        Precio = precio;
        StockAsignado = stockAsignado;
    }
    public ProductoProveedor() { }
}