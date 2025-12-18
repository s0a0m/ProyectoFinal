using src.Models.Domain;

namespace src.Interfaces;

public interface ICompraRepository
{
    Task<IEnumerable<Compra>> GetAllAsync();
    Task<Compra?> GetByIdAsync(int id);
    Task AddAsync(Compra compra);
    // void Update(Compra compra);
    // Task<bool> SaveChangesAsync();
}