using Microsoft.AspNetCore.Mvc.Rendering;
using src.Presentation.ViewModels.DepositoVM;

namespace src.Core.Services.Interfaces;

public interface IDepositoService
{
    Task<IEnumerable<DepositoListItemViewModel>> GetAllAsync();
    Task<DepositoFormViewModel>  GetFormularioVacioAsync();
    Task<DepositoFormViewModel?> GetFormularioAsync(int id);

    /// Recarga únicamente los select lists de un VM ya existente
    /// (útil después de un POST fallido para no perder los valores ingresados)
    Task CargarOpcionesAsync(DepositoFormViewModel vm);

    Task<(bool success, string message)> CreateAsync(DepositoFormViewModel vm);
    Task<(bool success, string message)> UpdateAsync(DepositoFormViewModel vm);
    Task<(bool success, string message)> DeleteAsync(int id);
    Task<(bool success, string message)> ReactivateAsync(int id);
    Task<DetalleDepositoViewModel?> GetDetalleAsync(int id);
    Task<IEnumerable<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>> GetProvinciasAsync();
}