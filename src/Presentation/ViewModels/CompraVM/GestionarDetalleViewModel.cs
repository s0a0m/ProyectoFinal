using System.ComponentModel.DataAnnotations;

namespace src.Presentation.ViewModels.CompraVM
{
    public class GestionarDetalleViewModel
    {
        public int IdDetalleCompra { get; set; }
        
        // Datos informativos (Solo lectura)
        public int IdProducto { get; set; }
        public string ProductoNombre { get; set; } = string.Empty;

        // Datos Editables con Validaciones
        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [Range(1, 10000, ErrorMessage = "La cantidad debe estar entre {1} y {2}.")]
        public int Cantidad { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0.")]
        public decimal PrecioPactado { get; set; }
        public bool Eliminar { get; set; } = false;
        // Calculado para la vista
        public decimal Subtotal => Cantidad * PrecioPactado;
    }
}