namespace src.Presentation.ViewModels.FacturaVM;

public class ListarDetalleFacturaViewModel
{
    public int IdFactura { get; set; }
    public string NumeroComprobante { get; set; } = null!;

    public DateTime Fecha { get; set; }
    public decimal Total { get; set; }
    public string EstadoPago { get; set; } = string.Empty;
    public string ProveedorNombre { get; set; } = string.Empty;
    public string CuitProveedor { get; set; } = string.Empty;
    public string CondicionPagoDesc { get; set; } = string.Empty;

    public List<ItemFacturaViewModel> Items { get; set; } = new();
}

public class ItemFacturaViewModel
{
    public string ProductoNombre { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal { get; set; }
}