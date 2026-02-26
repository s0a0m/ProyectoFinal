using System.ComponentModel.DataAnnotations;

namespace src.Presentation.ViewModels.FacturaVM
{
    public class CrearFacturaViewModel
    {
        [Required(ErrorMessage = "El número de compra es requerido")]
        public int IdCompra { get; set; }

        [Required(ErrorMessage = "El Proveedor es requerido")]
        public int IdProveedor { get; set; }

        public string RazonSocial { get; set; } = string.Empty;

        [Required(ErrorMessage = "El número de factura es requerido")]
        [StringLength(50, ErrorMessage = "El número no puede exceder los 50 caracteres.")]
        [Display(Name = "Número de Factura")]
        public string NumeroFactura { get; set; } = null!;

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Emisión")]
        public DateTime FechaEmision { get; set; } = DateTime.UtcNow;

        // --- Lógica de Condición de Pago ---
        [Required(ErrorMessage = "La forma de pago es requerida")]
        [Display(Name = "Tipo de Pago")]
        public string TipoCondicion { get; set; }  

        [Required(ErrorMessage = "La Dias de pago son requeridos")]
        [Range(0, 365, ErrorMessage = "Días de pago inválidos")]
        [Display(Name = "Días para el pago")]
        public short DiasPago { get; set; }

        // Solo para Cuotas
        [Range(1, 120, ErrorMessage = "Cantidad de cuotas inválida")]
        [Display(Name = "Cantidad de Cuotas")]
        public short? CantidadCuotas { get; set; }

        [Range(0, 100, ErrorMessage = "Interés inválido")]
        [Display(Name = "Interés (%)")]
        public decimal? InteresPorcentual { get; set; }

        // -----------------------------------

        public List<CrearFacturaDetalleViewModel> Detalles { get; set; } = new();
    }

    public class CrearFacturaDetalleViewModel
    {
        [Required(ErrorMessage = "El producto es requerido")]
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioBruto { get; set; }
        [Range(0, 100, ErrorMessage = "Porcentaje inválido")]
        public decimal PorcentajeDescuento { get; set; }
        public decimal PrecioNeto { get; set; } // El usuario puede editar esto si la factura difiere de la compra
        public decimal Subtotal => Cantidad * PrecioNeto;
    }
}