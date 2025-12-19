using src.Presentation.ViewModels.CompraVM;
using Dom = src.Models.Domain;

namespace src.Core.Services.Interfaces
{
    public interface IFacturaService
    {
        Task<IEnumerable<ListarFacturaViewModel>> ObtenerTodasAsync();
    }
}