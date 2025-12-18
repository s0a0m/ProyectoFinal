using System.ComponentModel.DataAnnotations;

namespace src.Presentation.ViewModels.CompraVM
{
    public class CrearCompraViewModel
    {
        [Required(ErrorMessage = "Debe seleccionar un proveedor.")]
        public int IdProveedor { get; set; }

        [StringLength(200, ErrorMessage = "Las observaciones no pueden superar los 200 caracteres.")]
        public string? Observaciones { get; set; }

        [Required(ErrorMessage = "La compra debe tener al menos un detalle.")]
        [MinLength(1, ErrorMessage = "Debe incluir al menos un producto en la compra.")]
        public List<CompraDetalleViewModel> Detalles { get; set; } = new();
    }
}