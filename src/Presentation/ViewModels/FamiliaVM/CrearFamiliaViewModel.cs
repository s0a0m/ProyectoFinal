using System.ComponentModel.DataAnnotations;

namespace src.Presentation.ViewModels.FamiliaVM
{
    public class CrearFamiliaViewModel
    {
        [Required(ErrorMessage = "El nombre de la familia es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder los 50 caracteres.")]
        [Display(Name = "Nombre de la Familia")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "La descripción no puede exceder los 200 caracteres.")]
        public string Descripcion { get; set; } = string.Empty;
    }
}