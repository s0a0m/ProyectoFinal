using System.ComponentModel.DataAnnotations;

namespace src.ViewModels
{
    public class CrearProveedorViewModel
    {
        [Required, RegularExpression(@"^\d{11}$", ErrorMessage = "El CUIT debe tener 11 dígitos numéricos.")]
        public string Cuit { get; set; }
        [Required, StringLength(100)]
        public string RazonSocial { get; set; }
        [Required, StringLength(12)]
        public string Telefono { get; set; }
        [Required, EmailAddress, StringLength(50)]
        public string Correo { get; set; }
        [Required, StringLength(80)]
        public string PersonaResponsable { get; set; }
        [Required, Range(0, 9999999999.99)]
        public decimal Saldo { get; set; }
        [Required]
        public CondicionDePagoViewModel CondicionPago { get; set; } = new();
        [Required]
        public DireccionViewModel Direccion { get; set; } = new();
    }
}