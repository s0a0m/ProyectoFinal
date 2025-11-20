using System.ComponentModel.DataAnnotations;

namespace src.Presentation.ViewModels.ProductoVM
{
    public class ActualizarProductoViewModel : CrearProductoViewModel
    {
        [Required]
        public int IdProducto { get; set; }
        
        public bool Activo { get; set; }
    }
}