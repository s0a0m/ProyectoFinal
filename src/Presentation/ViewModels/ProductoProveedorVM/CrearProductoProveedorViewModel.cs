using Microsoft.AspNetCore.Mvc.Rendering;
using src.Models.Domain;
using System.ComponentModel.DataAnnotations;
namespace src.Presentation.ViewModels.ProductoVM;
    public class CrearProductoProveedorViewModel
    {
        [Required(ErrorMessage = "Debe seleccionar un producto")]
        [Display(Name = "Producto")]
        public int IdProducto { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un proveedor")]
        [Display(Name = "Proveedor")]
        public int IdProveedor { get; set; }

        [Required]
        [Range(0.01, 99999999, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal Precio { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
        [Display(Name = "Stock Asignado")]
        public int StockAsignado { get; set; }

        [Display(Name = "Código de Barra del Proveedor")]
        public string CodigoBarraExterno { get; set; } = string.Empty;

        // Listas para los desplegables (Dropdowns)
        public List<SelectListItemDto> ListaProductos { get; set; } = new();
        public List<SelectListItemDto> ListaProveedores { get; set; } = new();
    }
