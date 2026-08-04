using Dom = src.Models.Domain;

namespace src.Repositories.Interfaces;

public interface IProvinciaRepository
{
    Task<IEnumerable<Dom.Provincia>> GetAllAsync();
    Task<Dom.Provincia?> GetByIdAsync(int idProvincia);
}