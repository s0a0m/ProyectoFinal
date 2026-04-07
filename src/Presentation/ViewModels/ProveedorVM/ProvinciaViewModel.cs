namespace src.Presentation.ViewModels.ProvinciaVM;

using System.ComponentModel.DataAnnotations;

public class ProvinciaViewModel
{
    [Required(ErrorMessage = "la provincia es obligatoria.")]
    [Range(1, 24, ErrorMessage = "Seleccione una provincia válida.")]
    public short Id_provincia { get; set; }

    public string? NombreProvincia { get; set; }
}

