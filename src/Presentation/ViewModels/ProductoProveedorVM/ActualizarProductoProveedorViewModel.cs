using Microsoft.AspNetCore.Mvc.Rendering;
using src.Models.Domain;
using System.ComponentModel.DataAnnotations;
namespace src.Presentation.ViewModels.ProductoVM;

    public class ActualizarProductoProveedorViewModel
    {
        // PKs (Ocultas o ReadOnly)
        public int IdProducto { get; set; }
        public int IdProveedor { get; set; }

        // Solo lectura para mostrar al usuario qué está editando
        [Display(Name = "Producto")]
        public string NombreProducto { get; set; } = string.Empty;

        [Display(Name = "Proveedor")]
        public string RazonSocialProveedor { get; set; } = string.Empty;

        [Display(Name = "Código Externo")]
        public List<string> CodigosBarraExternos{ get; set; } = new();
        // Editables
        [Required]
        [Range(0.01, 99999999)]
        public decimal Precio { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int StockAsignado { get; set; }
    }