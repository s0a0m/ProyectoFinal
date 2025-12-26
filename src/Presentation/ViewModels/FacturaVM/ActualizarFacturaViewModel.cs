using System.ComponentModel.DataAnnotations;

namespace src.Presentation.ViewModels.FacturaVM
{
    public class ActualizarFacturaViewModel
    {
        [Required]
        public int IdFactura { get; set; } // <--- CAMPO NUEVO CRÍTICO

        [Required]
        public int IdCompra { get; set; }

        [Required]
        public int IdProveedor { get; set; }

        public string RazonSocial { get; set; } = string.Empty;

        [Required(ErrorMessage = "El número de factura es obligatorio.")]
        [StringLength(50, ErrorMessage = "El número no puede exceder los 50 caracteres.")]
        [Display(Name = "Número de Factura")]
        public string NumeroFactura { get; set; } = null!;

        [Required(ErrorMessage = "La fecha de emisión es obligatoria.")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Emisión")]
        public DateTime FechaEmision { get; set; }

        // --- Lógica de Condición de Pago ---
        [Required]
        [Display(Name = "Tipo de Pago")]
        public string TipoCondicion { get; set; } = "Contado";

        [Required(ErrorMessage = "Los días de pago son obligatorios.")]
        [Range(0, 365, ErrorMessage = "Los días deben estar entre 0 y 365.")]
        [Display(Name = "Días Vencimiento")]
        public short DiasPago { get; set; }

        [Range(1, 120, ErrorMessage = "Las cuotas deben ser entre 1 y 120.")]
        [Display(Name = "Cant. Cuotas")]
        public short? CantidadCuotas { get; set; }

        [Range(0, 100, ErrorMessage = "El interés debe estar entre 0 y 100.")]
        [Display(Name = "Interés (%)")]
        public decimal? InteresPorcentual { get; set; }

        public List<CrearFacturaDetalleViewModel> Detalles { get; set; } = new();
    }
    // Reutilizamos la clase 'CrearFacturaDetalleViewModel' para los items ya que es idéntica
}