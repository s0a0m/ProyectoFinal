using System.ComponentModel.DataAnnotations;

namespace src.Presentation.ViewModels.FamiliaVM
{
    public class ActualizarFamiliaViewModel
    {
        [Required]
        public short IdFamilia { get; set; }

        [Required(ErrorMessage = "El nombre de la familia es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder los 50 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "La descripción no puede exceder los 200 caracteres.")]
        public string Descripcion { get; set; } = string.Empty;
    }
}