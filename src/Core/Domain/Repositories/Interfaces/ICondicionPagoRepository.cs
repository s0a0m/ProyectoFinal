using src.Models.Domain;

namespace src.Interfaces;

public interface ICondicionPagoRepository
{
    Task<IEnumerable<CondicionDePago>> GetAllAsync();
    Task<CondicionDePago?> GetByIdAsync(int id);
}