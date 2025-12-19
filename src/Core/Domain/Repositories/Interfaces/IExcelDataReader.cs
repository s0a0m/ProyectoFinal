using Dom = src.Models.Domain;
using src.Contracts;
namespace src.Repositories.Interfaces;

public interface IExcelDataReader
{
    IEnumerable<ProductoProveedorDataRow> ReadDataAsync(Stream fileStream, ImportacionColumnaMap mapaColumnas, bool contieneEncabezado = true, CancellationToken cancellationToken = default);
}