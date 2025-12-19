namespace src.Presentation.ViewModels.FacturaVM;

public class ListarDetalleFacturaViewModel
{
    public int IdFactura { get; set; }
    public string NumeroFactura { get; set; } = null!;
    public DateTime FechaEmision { get; set; }
    public decimal Total { get; set; }
    public bool Pagada { get; set; }
    public string ProveedorNombre { get; set; } = string.Empty;
    public string CondicionPago { get; set; } = string.Empty;

    public List<ItemFacturaViewModel> Items { get; set; } = new();
}

public class ItemFacturaViewModel
{
    public string Producto { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal => Cantidad * PrecioUnitario;
}