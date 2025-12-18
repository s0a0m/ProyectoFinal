using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace src.Presentation.ViewModels.NovedadesVM
{
    public class ResolverNovedadViewModel
    {
        [Required]
        public int IdNovedad { get; set; }

        // --- DATOS INFORMATIVOS (SOLO LECTURA DEL EXCEL) ---
        public string RazonSocialProveedor { get; set; } = string.Empty;
        public string CodigoBarraNovedad { get; set; } = string.Empty;
        
        [Display(Name = "Nombre en Excel")]
        public string? NombreSugerido { get; set; } // Puede venir vacío
        
        [Display(Name = "Precio en Excel")]
        public decimal? PrecioSugerido { get; set; } // Puede venir vacío

        // --- DATOS PARA LA RESOLUCIÓN (INPUTS) ---

        [Required(ErrorMessage = "Debe vincular esta novedad a un producto del sistema.")]
        [Display(Name = "Vincular con Producto")]
        public short IdProductoSeleccionado { get; set; }

        [Required(ErrorMessage = "El precio de costo es obligatorio para dar de alta.")]
        [Range(0.01, int.MaxValue, ErrorMessage = "El precio debe ser mayor a 0.")]
        [Display(Name = "Precio de Costo Final")]
        public decimal PrecioFinal { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo.")]
        [Display(Name = "Stock Inicial / Actual")]
        public int StockFinal { get; set; }

        // Lista desplegable
        public IEnumerable<SelectListItem>? ProductosDisponibles { get; set; }
    }
}