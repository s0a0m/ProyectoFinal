using Dom = src.Models.Domain;

namespace src.Repositories.Interfaces;

public interface IComprobanteRepository
{
    Task<IEnumerable<Dom.NotaCredito>> GetNotasCreditoAsync();
    Task<IEnumerable<Dom.NotaDebito>> GetNotasDebitoAsync();
    Task<Dom.Comprobante?> GetByIdAsync(int id);
    Task<IEnumerable<Dom.Comprobante>> GetByFacturaIdAsync(int idFactura);
    Task<IEnumerable<Dom.MotivoComprobante>> GetMotivosAsync(string? claseComprobante = null);
    Task<Dom.Comprobante> AddAsync(Dom.Comprobante comprobante);
    Task UpdateAsync(Dom.Comprobante comprobante);
    Task<bool> DeleteAsync(int id);
}