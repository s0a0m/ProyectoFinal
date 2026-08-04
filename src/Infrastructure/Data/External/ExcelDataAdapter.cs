using src.Contracts;
using ExcelDataReader;


namespace src.External;

public class ExcelDataAdapter : Repositories.Interfaces.IExcelDataReader
{
    public ExcelDataAdapter()
    {
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
    }

    public IEnumerable<ProductoProveedorDataRow> ReadDataAsync(Stream fileStream, ImportacionColumnaMap mapaColumnas, bool contieneEncabezado = true, CancellationToken cancellationToken = default)
    {
        return ReadDataWithYield(fileStream, mapaColumnas, contieneEncabezado, cancellationToken);
    }
    public IEnumerable<ProductoProveedorDataRow> ReadDataWithYield(
        Stream fileStream,
        ImportacionColumnaMap mapaColumnas,
        bool contieneEncabezado = true,
        CancellationToken cancellationToken = default)
    {
        IExcelDataReader? reader = null;
        using var nonClosingStream = new NonClosingStreamWrapper(fileStream);
        try
        {
            reader = ExcelReaderFactory.CreateReader(nonClosingStream);
        }
        catch (Exception)
        {
            throw new ArgumentException("El archivo no es un Excel válido o está corrupto.");
        }

        using (reader)
        {
            if (reader.Read())
            {
                int maxFieldCount = reader.FieldCount;
                if (!contieneEncabezado)
                {
                    yield return ProcesarFila(reader, mapaColumnas, maxFieldCount);
                }
            }
            else
            {
                yield break;
            }

            while (reader.Read())
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return ProcesarFila(reader, mapaColumnas, reader.FieldCount);
            }
        }
    }

    private ProductoProveedorDataRow ProcesarFila(IExcelDataReader reader, ImportacionColumnaMap mapa, int maxFieldCount)
    {
        return new ProductoProveedorDataRow
        {
            CodigoBarraExterno = GetSafeString(reader, mapa.CodigosBarrasExternosIndex, maxFieldCount) ?? string.Empty,
            NombreSugerido = GetSafeString(reader, mapa.NombreSugeridoIndex, maxFieldCount) ?? string.Empty,
            Precio = GetSafeDecimal(reader, mapa.PrecioIndex, maxFieldCount),
            StockActual = GetSafeInt(reader, mapa.StockIndex, maxFieldCount)
        };
    }
    private decimal GetSafeDecimal(IExcelDataReader reader, int index, int fieldCount)
    {
        if (index < 0 || index >= fieldCount || reader.IsDBNull(index)) return 0m;

        var value = reader.GetValue(index);

        if (value is double d) return (decimal)d;
        if (value is int i) return (decimal)i;
        if (value is decimal dec) return dec;

        if (value is string s && decimal.TryParse(s, out decimal parsed))
        {
            return parsed;
        }

        return 0m;
    }

    private string? GetSafeString(IExcelDataReader reader, int index, int fieldCount)
    {
        if (index < 0 || index >= fieldCount || reader.IsDBNull(index)) return null;

        var value = reader.GetValue(index);
        return value?.ToString()?.Trim();
    }

    private int GetSafeInt(IExcelDataReader reader, int index, int fieldCount)
    {
        if (index < 0 || index >= fieldCount || reader.IsDBNull(index)) return 0;

        var value = reader.GetValue(index);

        if (value is double d) return (int)d;
        if (value is int i) return i;

        if (value is string s && double.TryParse(s, out double parsedDouble))
        {
            return (int)parsedDouble;
        }

        return 0;
    }
}