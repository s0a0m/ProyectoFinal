using Dom = src.Models.Domain;

namespace src.Interfaces;

public interface ICondicionPagoRepository
{
    Task<IEnumerable<Dom.CondicionDePago>> GetAllAsync();
    Task<Dom.CondicionDePago?> GetByIdAsync(int id);
    Task<Dom.CondicionDePago> BuscarOCrearAsync(Dom.CondicionDePago condicion);
}