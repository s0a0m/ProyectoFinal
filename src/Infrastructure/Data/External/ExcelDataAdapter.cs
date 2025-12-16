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
        NonClosingStreamWrapper? nonClosingStream = null;
        int maxFieldCount = 0;

        try
        {
            if (fileStream.CanSeek)
            {
                fileStream.Seek(0, SeekOrigin.Begin);
            }

            nonClosingStream = new NonClosingStreamWrapper(fileStream);
            reader = ExcelReaderFactory.CreateReader(nonClosingStream);
            cancellationToken.ThrowIfCancellationRequested();
            if (reader.Read())
            {
                maxFieldCount = reader.FieldCount;
                if (!contieneEncabezado)
                {
                    // Si no hay encabezado, procesar la primera fila como datos
                    yield return new ProductoProveedorDataRow
                    {
                        CodigoBarraExterno = GetSafeString(reader, mapaColumnas.CodigosBarrasExternosIndex, maxFieldCount) ?? string.Empty,
                        NombreSugerido = GetSafeString(reader, mapaColumnas.NombreSugeridoIndex, maxFieldCount) ?? string.Empty,
                        Precio = GetSafeDecimal(reader, mapaColumnas.PrecioIndex, maxFieldCount),
                        StockActual = GetSafeInt(reader, mapaColumnas.StockIndex, maxFieldCount)
                    };
                    cancellationToken.ThrowIfCancellationRequested();
                }
            }
            else
            {
                yield break;
            }

            while (reader.Read())
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return new ProductoProveedorDataRow
                {
                    CodigoBarraExterno = GetSafeString(reader, mapaColumnas.CodigosBarrasExternosIndex, maxFieldCount) ?? string.Empty,
                    NombreSugerido = GetSafeString(reader, mapaColumnas.NombreSugeridoIndex, maxFieldCount) ?? string.Empty,
                    Precio = GetSafeDecimal(reader, mapaColumnas.PrecioIndex, maxFieldCount),
                    StockActual = GetSafeInt(reader, mapaColumnas.StockIndex, maxFieldCount)
                };
            }
        }
        finally
        {
            reader?.Dispose();
            nonClosingStream?.Dispose();
        }
    }

    public bool EsArchivoExcelValido(Stream fileStream)
    {
        using var nonClosingStream = new NonClosingStreamWrapper(fileStream);
        nonClosingStream.Seek(0, SeekOrigin.Begin);
        try
        {
            using var reader = ExcelReaderFactory.CreateReader(nonClosingStream);
            reader.Read();
            return true;
        }
        catch (ExcelDataReader.Exceptions.InvalidPasswordException ex)
        {
            System.Console.WriteLine($"Error de Encriptación: {ex.Message}");
            return false;
        }
        catch (ExcelDataReader.Exceptions.HeaderException ex)
        {
            System.Console.WriteLine($"Error de Formato/Encabezado: {ex.Message}");
            return false;
        }
        catch (ExcelDataReader.Exceptions.CompoundDocumentException ex)
        {
            System.Console.WriteLine($"Error de Documento Compuesto (.xls): {ex.Message}");
            return false;
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine($"Error al validar: {ex.Message}");
            return false;
        }
    }

    public int GetColumnCount(Stream fileStream)
    {
        if (fileStream.CanSeek) fileStream.Seek(0, SeekOrigin.Begin);
        using var nonClosingStream = new NonClosingStreamWrapper(fileStream);

        // Intenta crear el lector y avanzar a la primera hoja
        using var reader = ExcelReaderFactory.CreateReader(nonClosingStream);

        // El avance a la primera hoja/fila es necesario para obtener el FieldCount
        if (reader.Read())
        {
            return reader.FieldCount;
        }
        return 0; // Si no hay filas, el conteo es cero.
    }
    public int GetRowsCount(Stream fileStream)
    {
        if (fileStream.CanSeek) fileStream.Seek(0, SeekOrigin.Begin);
        using var nonClosingStream = new NonClosingStreamWrapper(fileStream);

        // Intenta crear el lector y avanzar a la primera hoja
        using var reader = ExcelReaderFactory.CreateReader(nonClosingStream);

        // El avance a la primera hoja/fila es necesario para obtener el FieldCount
        if (reader.Read())
        {
            int rowCount = 0;
            do
            {
                rowCount++;
            } while (reader.Read());
            return rowCount;
        }
        return 0; // Si no hay filas, el conteo es cero.
    }

    private decimal GetSafeDecimal(IExcelDataReader reader, int index, int fieldCount)
    {
        // 1. Control de Mapeo/Rango: Si el índice es -1 o fuera del FieldCount, devuelve 0m.
        if (index < 0 || index >= fieldCount)
        {
            return 0m;
        }

        try
        {
            // 2. Control de Nulo: Si es nulo en la DB, devuelve 0m.
            if (reader.IsDBNull(index))
            {
                return 0m;
            }

            // 3. Conversión: Lee como double (nativo) y convierte a Decimal.
            return Convert.ToDecimal(reader.GetDouble(index));
        }
        catch (System.InvalidCastException)
        {
            // Falla por texto en celda numérica.
            return 0m;
        }
        catch (System.Exception)
        {
            // Falla general de lectura.
            return 0m;
        }
    }

    private string? GetSafeString(IExcelDataReader reader, int index, int fieldCount)
    {
        if (index < 0 || index >= fieldCount)
        {
            return null;
        }

        try
        {
            if (reader.IsDBNull(index))
            {
                return null;
            }
            object cellValue = reader.GetValue(index);

            if (cellValue != null)
            {
                return cellValue.ToString()?.Trim();
            }
            return null;
        }
        catch (System.Exception)
        {
            // Falla de lectura (ej. dato binario inesperado).
            return null;
        }
    }

    private int GetSafeInt(IExcelDataReader reader, int index, int fieldCount)
    {
        if (index < 0 || index >= fieldCount) return 0;

        try
        {
            if (reader.IsDBNull(index)) return 0;

            // Lee como double, y luego intenta la conversión a int de forma segura
            double doubleValue = reader.GetDouble(index);

            // **Control explícito de Overflow:** Si el valor excede los límites de Int32.
            if (doubleValue < Int32.MinValue || doubleValue > Int32.MaxValue)
            {
                return 0; // Asigna 0 si hay overflow
            }

            return Convert.ToInt32(doubleValue);
        }
        catch (System.Exception)
        {
            // Falla por texto, nulo, etc.
            return 0;
        }
    }
}