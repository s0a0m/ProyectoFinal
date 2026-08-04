using System.ComponentModel.DataAnnotations;
using src.Models.CodeFirst;

namespace src.Presentation.ViewModels.ProductoVM
{
    public class ActualizarProductoViewModel : CrearProductoViewModel
    {
        [Required]
        public int IdProducto { get; set; }
        
        public bool Activo { get; set; }
        public List<string> CodigosRegistradosEnBd { get; set; } = new List<string>();
    }
}