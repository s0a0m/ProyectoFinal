using src.Presentation.ViewModels.EstanteVM;


namespace src.Core.Services.Interfaces;

public interface IEstanteService
{
    Task<IEnumerable<EstanteListItemViewModel>> GetByDepositoAsync(int idDeposito);
    Task<EstanteFormViewModel>  GetFormularioVacioAsync(int? idDeposito = null);
    Task<EstanteFormViewModel?> GetFormularioAsync(int id);
    Task CargarOpcionesAsync(EstanteFormViewModel vm);

    Task<(bool success, string message)> CreateAsync(EstanteFormViewModel vm);
    Task<(bool success, string message)> UpdateAsync(EstanteFormViewModel vm);
    Task<(bool success, string message)> DeleteAsync(int id);
    Task<(bool success, string message)> ReactivateAsync(int id);
    Task<(bool success, string message)> MarcarEspacioAsync(int id, bool tieneEspacio);
    Task SincronizarEspacioAsync(int idEstante);
}