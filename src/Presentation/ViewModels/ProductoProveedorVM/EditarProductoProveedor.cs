using Microsoft.AspNetCore.Mvc.Rendering;
using src.Models.Domain;
using System.ComponentModel.DataAnnotations;
namespace src.Presentation.ViewModels.ProductoVM;

public class EditarProductoProveedorVM
{
    public int IdProducto { get; set; }
    public int IdProveedor { get; set; }

    public decimal Precio { get; set; }
    public int StockAsignado { get; set; }

    public string CodigoBarraExterno { get; set; } = "";
}
