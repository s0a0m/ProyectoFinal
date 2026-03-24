using System.ComponentModel.DataAnnotations;

namespace src.Presentation.ViewModels.Comprobantes;

public class CrearComprobanteViewModel
{
    [Required]
    public int IdFactura { get; set; }

    [Required]
    public string TipoComprobante { get; set; } // "NC" o "ND"

    [StringLength(50)]
    public string Numero { get; set; } = string.Empty;

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "El total debe ser mayor a cero")]
    public decimal Total { get; set; }

    [Required]
    public short IdMotivo { get; set; }

    [StringLength(500)]
    public string? Comentario { get; set; }
}
