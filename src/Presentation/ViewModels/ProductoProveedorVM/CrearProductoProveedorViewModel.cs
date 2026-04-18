using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using src.Models.Domain;

namespace src.Presentation.ViewModels.ProductoVM;

public class CrearProductoProveedorViewModel
{
    [Required(ErrorMessage = "Debe seleccionar un producto")]
    [Display(Name = "Producto")]
    public int IdProducto { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un proveedor")]
    [Display(Name = "Proveedor")]
    public int IdProveedor { get; set; }

    [Required(ErrorMessage = "Debe ingresar el precio")]
    [Range(0.01, 99999999, ErrorMessage = "El precio debe de estar entre 0.01 y 99,999,999")]
    public decimal Precio { get; set; }

    // [Required(ErrorMessage = "Debe ingresar el stock")]
    // [Range(0, int.MaxValue, ErrorMessage = "El stock debe de estar entre 0 y 1,000,000,000")]
    // [Display(Name = "Stock Asignado")]
    // public int StockAsignado { get; set; }

    [Display(Name = "Código de Barra del Proveedor")]
    public List<string> CodigosBarraExternos { get; set; } = new();

    // Listas para los desplegables (Dropdowns)
    public List<SelectListItemDto> ListaProductos { get; set; } = new();
    public List<SelectListItemDto> ListaProveedores { get; set; } = new();
    public bool Activo { get; set; }
}
