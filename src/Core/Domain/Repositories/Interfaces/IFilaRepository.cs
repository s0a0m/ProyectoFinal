using Dom = src.Models.Domain;
public interface IFilaRepository
{
    Task<IEnumerable<Dom.Fila>> GetAllAsync();
    Task<IEnumerable<Dom.Fila>> GetByEstanteAsync(int idEstante);
    Task<Dom.Fila?> GetByIdAsync(int id);
    Task AddAsync(Dom.Fila entity);
    Task UpdateAsync(Dom.Fila entity);
    Task DeleteAsync(int id);
    Task ReactivateAsync(int id);
    Task<IEnumerable<Dom.Fila>> GetFilasConEspacioByDepositoAsync(int idDeposito);
}