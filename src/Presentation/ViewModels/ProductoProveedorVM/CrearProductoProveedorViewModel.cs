using Microsoft.AspNetCore.Mvc.Rendering;
using src.Models.Domain;
using System.ComponentModel.DataAnnotations;
namespace src.Presentation.ViewModels.ProductoVM;
public class CrearProductoProveedorVM
{
    public int IdProducto { get; set; }
    public int IdProveedor { get; set; }

    public decimal Precio { get; set; }
    public int StockAsignado { get; set; }

    public string CodigoBarraExterno { get; set; } = "";

    // Para combos
    public IEnumerable<SelectListItem> Productos { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> Proveedores { get; set; } = new List<SelectListItem>();
}
