using System.ComponentModel.DataAnnotations;
using Dom = src.Models.Domain; // Alias para el namespace de Dominio

namespace src.ViewModels
{
    public class ActualizarProveedorViewModel
    {
        [Required]
        public int IdProveedor { get; set; }


        [Required(ErrorMessage = "El CUIT es obligatorio.")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "El CUIT debe tener 11 dígitos numéricos.")]
        public string Cuit { get; set; } = string.Empty;

        [Required(ErrorMessage = "La razón social es obligatoria.")]
        [StringLength(100)]
        [RegularExpression(@"^[a-zA-Z0-9áéíóúÁÉÍÓÚñÑ\s.,&/-]+$", ErrorMessage = "La razón social contiene caracteres no permitidos")]
        public string RazonSocial { get; set; } = string.Empty;

        [Required(ErrorMessage = "El número de teléfono es obligatorio.")]
        [StringLength(12)]
        [Phone(ErrorMessage = "Formato de teléfono no válido.")]
        public string Telefono { get; set; } = string.Empty;

        [Required(ErrorMessage = "El Mail es obligatorio.")]
        [EmailAddress(ErrorMessage = "Formato de correo no válido."), StringLength(50)]
        public string Correo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre de la Persona responsable es obligatorio.")]
        [StringLength(80)]
        [RegularExpression(@"^[a-zA-Z0-9áéíóúÁÉÍÓÚñÑ\s.,&/-]+$", ErrorMessage = "El nombre de la persona contiene caracteres no permitidos")]
        public string PersonaResponsable { get; set; } = string.Empty;

        [Required(ErrorMessage = "El Saldo es obligatorio.")]
        [Range(0, 9999999999.99, ErrorMessage = "El saldo debe ser un valor positivo.")] // Mantener validación
        public decimal Saldo { get; set; }



        [Required(ErrorMessage = "La condición de pago es obligatoria.")]
        public CondicionDePagoViewModel CondicionPago { get; set; } = new();

        [Required(ErrorMessage = "La Dirección es obligatoria.")]
        public DireccionViewModel Direccion { get; set; } = new();

        /// <summary>
        /// Indica si el proveedor puede editar campos críticos (CUIT, Saldo Inicial).
        /// Se establece en false cuando el proveedor tiene facturas u órdenes de pago.
        /// </summary>
        public bool PuedeEditarIntegridad { get; set; } = true;

        // --- Constructores ---

        /// <summary>
        /// Constructor vacío necesario para el Model Binding en POST.
        /// </summary>
        public ActualizarProveedorViewModel() { }
    }
}
