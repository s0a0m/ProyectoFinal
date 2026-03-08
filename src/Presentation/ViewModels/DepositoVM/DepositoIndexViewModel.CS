// src/Models/ViewModels/Deposito/DepositoIndexViewModel.cs
namespace src.Presentation.ViewModels.DepositoVM;

public class DepositoIndexViewModel
{
    public IEnumerable<DepositoListItemViewModel> Depositos { get; set; }
        = Enumerable.Empty<DepositoListItemViewModel>();

    /// Formulario pre-cargado con la lista de provincias (usado tanto para
    /// crear como para editar; en editar se puebla vía JS + AJAX o hidden fields)
    public DepositoFormViewModel Formulario { get; set; } = new();
}