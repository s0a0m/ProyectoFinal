using src.Presentation.ViewModels.FilaVM;


namespace src.Core.Services.Interfaces;

public interface IFilaService
{
    Task<IEnumerable<FilaListItemViewModel>> GetByEstanteAsync(int idEstante);
    Task<FilaFormViewModel>  GetFormularioVacioAsync(int? idEstante = null);
    Task<FilaFormViewModel?> GetFormularioAsync(int id);
    Task CargarOpcionesAsync(FilaFormViewModel vm);

    Task<(bool success, string message)> CreateAsync(FilaFormViewModel vm);
    Task<(bool success, string message)> UpdateAsync(FilaFormViewModel vm);
    Task<(bool success, string message)> DeleteAsync(int id);
    Task<(bool success, string message)> ReactivateAsync(int id);
}