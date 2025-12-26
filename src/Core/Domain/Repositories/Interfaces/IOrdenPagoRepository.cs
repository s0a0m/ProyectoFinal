using Dom = src.Models.Domain;

namespace src.Repositories.Interfaces;

public interface IOrdenPagoRepository
{
    Task<IEnumerable<Dom.OrdenPago>> GetAllAsync();
    Task<Dom.OrdenPago?> GetByIdWithDetallesAsync(int id);
    Task<IEnumerable<Dom.OrdenPago>> GetPagosPorFacturaIdAsync(int idFactura);
    Task CreateAsync(Dom.OrdenPago orden);
    Task UpdateAsync(Dom.OrdenPago orden);
}