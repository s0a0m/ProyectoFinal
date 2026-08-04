using System.ComponentModel.DataAnnotations;

namespace src.Presentation.ViewModels.CompraVM
{
    public class CompraDetalleViewModel
    {
        [Required(ErrorMessage = "El ID del producto es obligatorio.")]
        public int IdProducto { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1.")]
        public int Cantidad { get; set; }

        [Required(ErrorMessage = "El precio pactado es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0.")]
        public decimal PrecioPactado { get; set; }
    }
}