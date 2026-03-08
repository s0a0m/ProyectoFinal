using Dom = src.Models.Domain;
public interface IEstanteRepository
{
    Task<IEnumerable<Dom.Estante>> GetAllAsync();
    Task<IEnumerable<Dom.Estante>> GetByDepositoAsync(int idDeposito);
    Task<Dom.Estante?> GetByIdAsync(int id);
    Task AddAsync(Dom.Estante entity);
    Task UpdateAsync(Dom.Estante entity);
    Task MarcarEstanteLlenoAsync(int idEstante, bool tieneEspacio);
    Task DeleteAsync(int id);
    Task ReactivateAsync(int id);
    Task SincronizarEspacioAsync(int idEstante);
}