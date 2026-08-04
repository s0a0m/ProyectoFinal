using System.ComponentModel.DataAnnotations;

namespace src.Presentation.ViewModels.CategoriaVM
{
    public class CrearCategoriaViewModel
    {
        // Es crucial recibir el ID de la familia padre para crear la relación 1-N
        [Required(ErrorMessage = "Debe seleccionar una familia padre.")]
        public short IdFamilia { get; set; }

        [Required(ErrorMessage = "El nombre de la categoría es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder los 50 caracteres.")]
        [Display(Name = "Nombre de la Categoría")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "La descripción no puede exceder los 100 caracteres.")]
        public string Descripcion { get; set; } = string.Empty;
    }
}