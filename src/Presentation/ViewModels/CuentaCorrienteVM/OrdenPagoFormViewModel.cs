using System.ComponentModel.DataAnnotations;

namespace src.Presentation.ViewModels.CuentaCorrienteVM
{ 
    public class OrdenPagoFormVM : IValidatableObject
    {
        public int IdOrdenPago { get; set; }
        public int IdProveedor { get; set; }
        public string RazonSocialProveedor { get; set; } = string.Empty;

        [Required(ErrorMessage = "El número de orden es obligatorio.")]
        [StringLength(50, ErrorMessage = "El número no puede exceder los 50 caracteres.")]
        [Display(Name = "Número de Orden")]
        public string NumeroOrden { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de pago es obligatoria.")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Pago")]
        public DateTime FechaPago { get; set; } = DateTime.UtcNow;

        public List<SeleccionFacturaVM> FacturasDisponibles { get; set; } = new List<SeleccionFacturaVM>();

        // Validaciones complejas del lado del servidor
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // 1. Validar que se haya seleccionado al menos una factura con monto > 0
            var itemsSeleccionados = FacturasDisponibles.Where(x => x.EstaSeleccionada).ToList();

            if (!itemsSeleccionados.Any())
            {
                yield return new ValidationResult(
                    "Debe seleccionar al menos una factura para realizar el pago.", 
                    new[] { nameof(FacturasDisponibles) });
            }

            // 2. Validar consistencia de montos (Seguridad extra al Front)
            foreach (var item in itemsSeleccionados)
            {
                if (item.MontoAPagar <= 0)
                {
                    yield return new ValidationResult(
                        $"El monto a pagar para la factura {item.NumeroFactura} debe ser mayor a 0.");
                }

                if (item.MontoAPagar > item.SaldoPendiente)
                {
                    yield return new ValidationResult(
                        $"El monto a pagar (${item.MontoAPagar}) para la factura {item.NumeroFactura} supera el saldo pendiente (${item.SaldoPendiente}).");
                }
            }
        }
    }

    public class SeleccionFacturaVM
    {
        public int IdFactura { get; set; }
        public string NumeroFactura { get; set; } = string.Empty;
        public DateTime FechaEmision { get; set; }
        public decimal TotalFacturado { get; set; }
        public decimal SaldoPendiente { get; set; }

        public bool EstaSeleccionada { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "El monto debe ser válido.")]
        public decimal MontoAPagar { get; set; }
    }
}
