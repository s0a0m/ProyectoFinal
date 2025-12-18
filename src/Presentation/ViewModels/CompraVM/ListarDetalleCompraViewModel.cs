namespace src.Presentation.ViewModels.CompraVM;

public class ListarDetalleCompraViewModel
{
    public int IdDetalleCompra { get; set; }
    public int IdProducto { get; set; }
    public string ProductoNombre { get; set; } = string.Empty;
    // public List<string> ProductoCodigo { get; set; } = new();
    public int Cantidad { get; set; }
    public decimal PrecioPactado { get; set; }
    public decimal Subtotal => Cantidad * PrecioPactado;
}