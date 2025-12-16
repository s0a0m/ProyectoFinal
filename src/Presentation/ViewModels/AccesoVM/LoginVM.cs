using System.ComponentModel.DataAnnotations;
namespace Presentation.ViewModels.AccesoVM
{
    public class LoginVM
    {
        [Required(ErrorMessage = "El correo es obligatorio")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        public string Clave { get; set; }
    }
}