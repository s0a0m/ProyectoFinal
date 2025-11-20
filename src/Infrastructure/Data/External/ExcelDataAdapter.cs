using src.Contracts;
using ExcelDataReader;


namespace src.External;

public class ExcelDataAdapter : Repositories.Interfaces.IExcelDataReader
{
    public ExcelDataAdapter()
    {
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
    }

    public Task<IEnumerable<ProductoProveedorDataRow>> ReadDataAsync(Stream fileStream, ImportacionColumnaMap mapaColumnas)
    {
        return Task.Run(() => ReadDataWithYield(fileStream, mapaColumnas));
    }
    private IEnumerable<ProductoProveedorDataRow> ReadDataWithYield(
        Stream fileStream,
        ImportacionColumnaMap mapaColumnas)
    {
        fileStream.Seek(0, SeekOrigin.Begin);
        using var reader = ExcelReaderFactory.CreateReader(fileStream);
        if (reader.Read())
        {
            while (reader.Read())
            {
                yield return new ProductoProveedorDataRow
                {
                    CodigoBarraExterno = reader.GetString(mapaColumnas.CodigosBarrasExternosIndex),
                    NombreSugerido = reader.GetString(mapaColumnas.NombreSugeridoIndex),
                    Precio = Convert.ToDecimal(reader.GetDouble(mapaColumnas.PrecioIndex)),
                    StockActual = (int)reader.GetDouble(mapaColumnas.StockIndex)
                };
            }
        }
    }
}