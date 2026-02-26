using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace src.Presentation.ViewModels.NovedadesVM
{
    public class ResolverNovedadViewModel
    {
        [Required]
        public int IdNovedad { get; set; }

        // --- DATOS INFORMATIVOS (SOLO LECTURA - ORIGEN EXCEL) ---

        public short IdProveedor { get; set; } // Agregado: Útil para validar o redirigir

        [Display(Name = "Proveedor")]
        public string RazonSocialProveedor { get; set; } = string.Empty;

        [Display(Name = "Código")]
        public string CodigoBarraNovedad { get; set; } = string.Empty;

        [Display(Name = "Nombre en Excel")]
        public string? NombreSugerido { get; set; }

        [Display(Name = "Precio en Excel")]
        public decimal? PrecioSugerido { get; set; }

        // --- FALTA ESTE CAMPO (CRÍTICO PARA LA VISTA) ---
        [Display(Name = "Stock en Excel")]
        public int StockSugerido { get; set; }


        // --- DATOS PARA LA RESOLUCIÓN (INPUTS DE USUARIO) ---

        [Required(ErrorMessage = "Debe buscar y seleccionar un producto del sistema.")]
        [Range(1, short.MaxValue, ErrorMessage = "Seleccione un producto válido.")]
        [Display(Name = "Producto del Sistema")]
        public short IdProductoSeleccionado { get; set; }

        [Required(ErrorMessage = "El precio final es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0.")]
        [Display(Name = "Precio de Costo Final")]
        public decimal PrecioFinal { get; set; }

        [Required(ErrorMessage = "El stock es obligatorio.")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo.")]
        [Display(Name = "Stock a Asignar")]
        public int StockFinal { get; set; }

        // Lista para el Buscador (Select2)
        public IEnumerable<SelectListItem>? ProductosDisponibles { get; set; }
    }
}