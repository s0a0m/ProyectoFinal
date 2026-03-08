using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace src.Presentation.ViewModels.FilaVM;


public class FilaFormViewModel
{
    public int IdFila { get; set; }

    [Required(ErrorMessage = "El nombre / número de la fila es obligatorio.")]
    [StringLength(50)]
    [Display(Name = "Nombre de fila")]
    public string NFila { get; set; } = string.Empty;

    public bool Activo       { get; set; } = true;
    public bool TieneEspacio { get; set; } = true;

    [StringLength(500)]
    [Display(Name = "Observaciones")]
    public string? Observaciones { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un estante.")]
    [Display(Name = "Estante")]
    public int IdEstante { get; set; }

    public string NumeroEstante  { get; set; } = string.Empty;
    public int    IdDeposito     { get; set; }
    public string NombreDeposito { get; set; } = string.Empty;

    public IEnumerable<SelectListItem> Estantes { get; set; }
        = Enumerable.Empty<SelectListItem>();
}