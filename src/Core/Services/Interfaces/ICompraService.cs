using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Common;
using src.Core.Contracts;
using src.Presentation.ViewModels.CompraVM;
using Dom = src.Models.Domain;

namespace src.Core.Services.Interfaces
{
    public interface ICompraService
    {
        Task<IEnumerable<ListarCompraViewModel>> GetAllAsync();
        Task<ListarCompraViewModel?> GetByIdAsync(short id);
        Task<int> CreateAsync(Dom.Compra compra);
        Task UpdateAsync(Dom.Compra compra);
        Task CancelarCompraAsync(short id);
        Task EsEditableAsync(short id);
        Task<Dom.Compra?> ObtenerPorIdAsync(int id);
    }
}
