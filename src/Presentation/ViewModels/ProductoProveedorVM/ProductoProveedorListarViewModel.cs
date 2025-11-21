using System.ComponentModel.DataAnnotations;

namespace src.Presentation.ViewModels.ProductoVM;

public class ProductoProveedorListarViewModel
{
    public int IdProducto { get; set; }
    public int IdProveedor { get; set; }
    public string NombreProducto { get; set; } = "";
    public string NombreProveedor { get; set; } = "";
    public string CodigoBarraExterno { get; set; } = "";
    public decimal Precio { get; set; }
    public int StockAsignado { get; set; }
}