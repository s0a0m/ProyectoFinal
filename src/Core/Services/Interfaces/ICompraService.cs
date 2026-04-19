using Core.Common;
using Dom = src.Models.Domain;

namespace src.Core.Services.Interfaces
{
    public interface ICompraService
    {
        Task<IEnumerable<Dom.Compra>> GetAllAsync();
        Task<ServiceResult<Dom.Compra>> GetByIdAsync(int id);
        Task<ServiceResult> UpdateAsync(Dom.Compra compraEditada);
        Task<ServiceResult<int>> ProcesarCompraDesdeCarritoAsync(
            short idProveedor,
            int idUsuario,
            string observaciones
        );
        ServiceResult ValidarMontoTotal(decimal totalCalculado);
    }
}
