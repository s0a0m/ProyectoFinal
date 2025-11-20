using Dom = src.Models.Domain;
using src.Contracts;
namespace src.Repositories.Interfaces;

public interface IExcelDataReader
{
    Task<IEnumerable<ProductoProveedorDataRow>> ReadDataAsync(Stream fileStream, ImportacionColumnaMap mapaColumnas);
}