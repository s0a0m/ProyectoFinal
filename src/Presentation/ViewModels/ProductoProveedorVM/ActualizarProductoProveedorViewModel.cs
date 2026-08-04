using Microsoft.AspNetCore.Mvc.Rendering;
using src.Models.Domain;
using System.ComponentModel.DataAnnotations;
namespace src.Presentation.ViewModels.ProductoVM;

    public class ActualizarProductoProveedorViewModel
    {
        
        public int IdProducto { get; set; }
        public int IdProveedor { get; set; }

        // Solo lectura 
        [Display(Name = "Producto")]
        public string NombreProducto { get; set; } = string.Empty;

        [Display(Name = "Proveedor")]
        public string RazonSocialProveedor { get; set; } = string.Empty;

        [Display(Name = "Código Externo")]
        public List<string> CodigosBarraExternos{ get; set; } = new();
        // Editables
         [Display(Name = "Nuevos Códigos de Barra")]
        public List<string> NuevosCodigosBarraExternos { get; set; } = new();
        
        [Required(ErrorMessage = "Debe ingresar el precio")]
        [Range(0.01, 99999999, ErrorMessage = "El precio debe tener un valor positivo y valido")]
        public decimal Precio { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "El stock asignado debe de ser un número positivo y valido")]
        public int StockAsignado { get; set; }
        public bool Activo { get; set; }
    }