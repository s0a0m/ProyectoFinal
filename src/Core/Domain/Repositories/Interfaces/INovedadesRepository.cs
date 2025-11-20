using src.Contracts;
namespace src.Repositories.Interfaces;

public interface INovedadesRepository
{
    Task<IEnumerable<NovedadPendiente>> GetPendientesAsync();
    Task AddAsync(NovedadPendiente entity);
}