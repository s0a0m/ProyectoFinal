 using Dom = src.Models.Domain;

namespace src.Repositories.Interfaces;   
public interface IDepositoRepository
{
    Task<IEnumerable<Dom.Deposito>> GetAllAsync();
    Task<Dom.Deposito?> GetByIdAsync(int id);
    Task AddAsync(Dom.Deposito entity);
    Task UpdateAsync(Dom.Deposito entity);
}