using System.ComponentModel.DataAnnotations;
using src.Presentation.ViewModels.CategoriaVM; // O donde tengas un VM simple para dropdowns

namespace src.Presentation.ViewModels.ProductoVM
{
    public class CategoriaOpcionDto
    {
        public short Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Familia { get; set; } = string.Empty; // Ya separado
    }
    public class CrearProductoViewModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(50)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [Range(0, int.MaxValue)]
        [Display(Name = "Stock Mínimo")]
        public int StockMinimo { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        [Display(Name = "Stock Inicial")]
        public int StockTotal { get; set; }

        // RF 2.3.1 - Al menos un código (Validación personalizada o en controlador)
        [Required(ErrorMessage = "Debe ingresar al menos un código de barras.")]
        [MinLength(1, ErrorMessage = "Debe haber al menos un código de barras.")]
        public List<string> CodigosBarra { get; set; } = new List<string>();

        // Selección de Categorías (N-N)
        [Required(ErrorMessage = "Seleccione al menos una categoría.")]
        public List<short> IdsCategoriasSeleccionadas { get; set; } = new List<short>();

        // Para llenar los selectores en la vista (Dropdowns)

        public IEnumerable<CategoriaOpcionDto>? ListaCategoriasDisponibles { get; set; }
    }
}