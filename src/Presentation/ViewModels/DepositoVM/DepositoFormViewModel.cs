using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace src.Presentation.ViewModels.DepositoVM;

public class DepositoFormViewModel
{
    public int IdDeposito { get; set; }

    [Required(ErrorMessage = "El nombre del depósito es obligatorio.")]
    [StringLength(100, ErrorMessage = "El nombre no puede superar 100 caracteres.")]
    [Display(Name = "Nombre del depósito")]
    public string Nombre { get; set; } = string.Empty;

    // ── Dirección ──────────────────────────────────────────────────────────────
    [Required(ErrorMessage = "La calle es obligatoria.")]
    [StringLength(100)]
    [Display(Name = "Calle")]
    public string Calle { get; set; } = string.Empty;

    // short en el dominio; el input HTML es numérico, se valida rango 1-99999
    [Required(ErrorMessage = "El número es obligatorio.")]
    [Range(1, short.MaxValue, ErrorMessage = "El número debe estar entre 1 y 32767.")]
    [Display(Name = "Número")]
    public short Numero { get; set; }

    [Range(0, short.MaxValue, ErrorMessage = "El piso debe ser un número positivo.")]
    [Display(Name = "Piso / Depto")]
    public short? Piso { get; set; }

    [StringLength(300)]
    [Display(Name = "Comentario / Referencias")]
    public string? Comentario { get; set; }

    // ── Provincia ──────────────────────────────────────────────────────────────
    [Required(ErrorMessage = "Debe seleccionar una provincia.")]
    [Display(Name = "Provincia")]
    public short IdProvincia { get; set; }

    public bool Activo { get; set; } = true;

    // ── Select list ────────────────────────────────────────────────────────────
    public IEnumerable<SelectListItem> Provincias { get; set; }
        = Enumerable.Empty<SelectListItem>();
}