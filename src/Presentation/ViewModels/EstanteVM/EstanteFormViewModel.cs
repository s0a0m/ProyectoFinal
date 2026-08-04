using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace src.Presentation.ViewModels.EstanteVM;

public class EstanteFormViewModel
{
    public int IdEstante { get; set; }

    [Required(ErrorMessage = "El número / nombre del estante es obligatorio.")]
    [StringLength(50)]
    [Display(Name = "Número de estante")]
    public string NumeroEstante { get; set; } = string.Empty;

    public bool Activo       { get; set; } = true;
    public bool TieneEspacio { get; set; } = true;

    [StringLength(500)]
    [Display(Name = "Observaciones")]
    public string? Observaciones { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un depósito.")]
    [Display(Name = "Depósito")]
    public int IdDeposito { get; set; }

    public string NombreDeposito { get; set; } = string.Empty;

    public IEnumerable<SelectListItem> Depositos { get; set; }
        = Enumerable.Empty<SelectListItem>();
}