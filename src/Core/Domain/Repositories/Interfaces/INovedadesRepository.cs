using src.Contracts;
namespace src.Repositories.Interfaces;

public interface INovedadesRepository
{
    Task<IEnumerable<NovedadPendiente>> GetPendientesAsync();
    Task<int> ContarPendientesAsync();
    Task AddAsync(NovedadPendiente entity, CancellationToken cancellationToken = default);
}