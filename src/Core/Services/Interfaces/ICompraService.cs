using System.Collections.Generic;
using System.Threading.Tasks;
using src.Presentation.ViewModels.CompraVM;
using Dom = src.Models.Domain;

namespace src.Core.Services.Interfaces
{
    public interface ICompraService
    {
        Task<IEnumerable<ListarCompraViewModel>> GetAllAsync();
        Task<ListarCompraViewModel?> GetByIdAsync(short id);
        Task<int> CreateAsync(CrearCompraViewModel compra, short idUsuario);
        Task UpdateAsync(Dom.Compra compra);
        Task CancelarCompraAsync(short id);
        Task EsEditableAsync(short id);
    }
}