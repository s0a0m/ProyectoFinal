using System.ComponentModel.DataAnnotations;

namespace src.ViewModels
{
    public class CrearProveedorViewModel
    {
        [Required, RegularExpression(@"^\d{11}$", ErrorMessage = "El CUIT debe tener 11 dígitos numéricos.")]
        public string Cuit { get; set; }

        [Required(ErrorMessage = "La razon social es obligatoria.")]
        [StringLength(100)]
        public string RazonSocial { get; set; }

        [Required(ErrorMessage = "El numero de telefono es obligatorio.")]
        [StringLength(12)]
        public string Telefono { get; set; }


        [Required, EmailAddress, StringLength(50)]
        public string Correo { get; set; }

        [Required(ErrorMessage = "El nombre de la Persona responsable es obligatorio.")]
        [StringLength(80)]
        public string PersonaResponsable { get; set; }

        [Required(ErrorMessage = "El Saldo es obligatorio.")]
        [ Range(0, 9999999999.99)]
        public decimal Saldo { get; set; }

        
        [Required(ErrorMessage = "La condicion de pago es obligatoria.")]
        public CondicionDePagoViewModel CondicionPago { get; set; } = new();


        [Required(ErrorMessage = "La Direccion es obligatoria.")]
        public DireccionViewModel Direccion { get; set; } = new();
    }
}