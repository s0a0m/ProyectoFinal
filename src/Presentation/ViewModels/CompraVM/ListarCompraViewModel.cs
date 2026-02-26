namespace src.Presentation.ViewModels.CompraVM;

public class ListarCompraViewModel
{
    public int IdCompra { get; set; }
    public DateTime Fecha { get; set; }
    public string Estado { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public int IdProveedor { get; set; }
    public string ProveedorRazonSocial { get; set; } = string.Empty;
    public string? Observaciones { get; set; }
    public string UsuarioNombre { get; set; } = string.Empty;
    public string UsuarioApellido { get; set; } = string.Empty;
    public List<ListarDetalleCompraViewModel> Detalles { get; set; } = new();
}