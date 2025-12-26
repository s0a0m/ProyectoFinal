using System.Collections.Generic;
using System.Threading.Tasks;
using Dom = src.Models.Domain;

namespace src.Repositories.Interfaces
{
    public interface IFacturaRepository
    {
        Task<Dom.Factura?> GetByIdAsync(int id);
        Task<IEnumerable<Dom.Factura>> GetAllAsync();
        Task<IEnumerable<Dom.Factura>> GetByProveedorAsync(short idProveedor);
        // Task<IEnumerable<Dom.Factura>> GetByRangoFechaAsync(DateTime desde, DateTime hasta);
        Task<IEnumerable<Dom.Factura>> GetPendientesPagoAsync();
        Task<Dom.Factura> AddAsync(Dom.Factura factura);
        Task UpdateAsync(Dom.Factura factura);
        Task<bool> ExisteNumeroFacturaAsync(short idProveedor, string numeroFactura);
        Task<bool> ExisteNumeroFacturaAsync(short idProveedor, string numero, int? idExcluir = null);

        Task ActualizarSaldoYEstadoAsync(int idFactura, decimal nuevoSaldo, bool pagada,DateTime FechaPago);
    }
}