namespace src.Models.Domain;

public class DetalleCompra
{
    public int IdDetalleCompra { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioPactado { get; set; }
    public decimal Subtotal => Cantidad * PrecioPactado;
    public Producto Producto { get; set; } = new();
    public Compra Compra { get; set; } = new();
}