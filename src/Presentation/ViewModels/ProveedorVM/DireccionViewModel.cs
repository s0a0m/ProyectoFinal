using System.ComponentModel.DataAnnotations;
using src.Presentation.ViewModels.ProvinciaVM;
using Dom = src.Models.Domain;

namespace src.Presentation.ViewModels.DireccionVM;

public class DireccionViewModel
{
    public ProvinciaViewModel provincia { get; set; } = new();

    [Required(ErrorMessage = "El nombre de la calle es obligatorio.")]
    [StringLength(100)]
    public string calle { get; set; } = string.Empty;

    [Required(ErrorMessage = "El numero de calle es obligatorio.")]
    [Range(1, 9999, ErrorMessage = "El numero de calle debe estar entre 1 y 9999")]
    public short numero { get; set; }

    [Range(1, 200)]
    public short? piso { get; set; }

    [StringLength(1000)]
    public string? comentario { get; set; }
    public List<Dom.Provincia> ListaProvincias { get; set; } = new List<Dom.Provincia>();
}
