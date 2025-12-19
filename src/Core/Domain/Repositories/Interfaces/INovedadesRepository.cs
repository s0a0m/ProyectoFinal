using src.Contracts;
using src.Models.CodeFirst;
namespace src.Repositories.Interfaces;

public interface INovedadesRepository
{
    Task<IEnumerable<NovedadPendiente>> GetPendientesAsync();
    Task<int> ContarPendientesAsync();
    Task AddAsync(NovedadPendiente entity, CancellationToken cancellationToken = default);
    Task<NovedadPendiente?> GetByIdAsync(int id);
    Task UpdateAsync(NovedadPendiente entity);
    Task<IDictionary<string, NovedadesProveedor>> ObtenerPendientesPorCodigosAsync(
        List<string> codigos,
        short idProveedor,
        CancellationToken ct);
    Task AddSinGuardarAsync(NovedadesProveedor novedad, CancellationToken ct);
}