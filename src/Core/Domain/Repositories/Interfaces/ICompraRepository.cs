using src.Models.Domain;

namespace src.Repositories.Interfaces;

public interface ICompraRepository
{
    Task<IEnumerable<Compra>> GetAllAsync();
    Task<Compra?> GetByIdAsync(int id);
    Task AddAsync(Compra compra);
    Task UpdateAsync(Compra compra);
    Task FinalizarCompraAsync(int idCompra);
    Task CancelarCompraAsync(int idCompra, string motivo);
    Task MarcarComoEnviadaAsync(int idCompra);
    Task CompletarCompraAsync(int idCompra);
}

