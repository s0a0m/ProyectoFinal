namespace src.Models.Domain;

public class ProductoProveedor
{
    public Producto Producto { get; set; }
    public Proveedor Proveedor { get; set; }
    public decimal Precio { get; set; }
    public bool Activo { get; set; }

    public ProductoProveedor() { }
}

