using System.ComponentModel.DataAnnotations;

namespace src.Presentation.ViewModels.CompraVM
{
    public class GestionarCompraViewModel
    {
        [Required]
        public int IdCompra { get; set; }

        // --- Datos de Cabecera (Solo Lectura) ---
        public DateTime Fecha { get; set; }
        public string Estado { get; set; } = string.Empty;
        public decimal TotalEstimado => Detalles.Sum(x => x.Subtotal);
        
        public int IdProveedor { get; set; }
        public string ProveedorRazonSocial { get; set; } = string.Empty;
        
        public string UsuarioNombreCompleto { get; set; } = string.Empty;

        // --- Datos Editables ---
        [Display(Name = "Observaciones")]
        [StringLength(500, ErrorMessage = "Las observaciones no pueden exceder los 500 caracteres.")]
        public string? Observaciones { get; set; }

        // --- Lista de Detalles ---
        public List<GestionarDetalleViewModel> Detalles { get; set; } = new();
    }
}