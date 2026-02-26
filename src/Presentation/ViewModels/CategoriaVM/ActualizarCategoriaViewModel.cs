using System.ComponentModel.DataAnnotations;

namespace src.Presentation.ViewModels.CategoriaVM
{
    public class ActualizarCategoriaViewModel
    {
        [Required]
        public short IdCategoria { get; set; }

        [Required(ErrorMessage = "La categoría debe pertenecer a una familia.")]
        public short IdFamilia { get; set; }

        [Required(ErrorMessage = "El nombre de la categoría es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder los 50 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "La descripción no puede exceder los 100 caracteres.")]
        public string Descripcion { get; set; } = string.Empty;
    }
}