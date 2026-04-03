 using Dom = src.Models.Domain;

namespace src.Repositories.Interfaces;   
public interface IDepositoRepository
{
    Task<IEnumerable<Dom.Deposito>> GetAllAsync();
    Task<Dom.Deposito?> GetByIdAsync(int id);
    Task AddAsync(Dom.Deposito entity);
    Task UpdateAsync(Dom.Deposito entity);
    Task DeleteAsync(int id);
    Task ReactivateAsync(int id);
    Task<IEnumerable<Dom.Deposito>> GetActivosAsync();
    Task<Dictionary<int, (int Estantes, int Filas)>> GetConteosAsync();

}