using src.ViewModels;
using Dom = src.Models.Domain;
using src.Contracts;
namespace src.Core.Services.Interfaces;

public interface IProductoProveedorService
{
    Task<IEnumerable<AccionDeFilaCargaAutomatica>> ProcesarListaDePreciosAsync(Stream fileStream, short idProveedor, ImportacionColumnaMap mapaColumnas);
}