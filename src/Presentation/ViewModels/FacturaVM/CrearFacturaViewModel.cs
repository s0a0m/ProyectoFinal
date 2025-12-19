using System.ComponentModel.DataAnnotations;

namespace src.Presentation.ViewModels.FacturaVM
{
    public class CrearFacturaViewModel
    {
        [Required(ErrorMessage = "La compra vinculada es obligatoria")]
        public int IdCompra { get; set; }

        [Required(ErrorMessage = "El proveedor es obligatorio")]
        public short IdProveedor { get; set; }

        [Required(ErrorMessage = "El número de factura es requerido")]
        [StringLength(50)]
        public string NumeroFactura { get; set; } = null!;

        [Required]
        public DateTime FechaEmision { get; set; } = DateTime.Now;

        [Required]
        public short IdCondicionPago { get; set; }

        public List<CrearFacturaDetalleViewModel> Detalles { get; set; } = new();
    }

    public class CrearFacturaDetalleViewModel
    {
        public short IdProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }
}