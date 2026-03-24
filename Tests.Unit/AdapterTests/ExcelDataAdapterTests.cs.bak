using src.External;
using src.Contracts;

namespace src.AdapterTest;

public class ExcelDataAdapterTests
{
    private readonly ExcelDataAdapter _adapter = new ExcelDataAdapter();
    private const string NombreArchivoMock = "mock_proveedor_datos.xlsx";
    private readonly ImportacionColumnaMap _mapa = new ImportacionColumnaMap
    {
        CodigosBarrasExternosIndex = 0,
        NombreSugeridoIndex = 1,
        PrecioIndex = 2,
        StockIndex = 3
    };

    [Fact]
    public async Task ReadDataAsync_LecturaArchivoReal_DebeMapearTresFilas()
    {
        string rutaCompleta = Path.Combine(AppContext.BaseDirectory, "AdapterTests", NombreArchivoMock);

        using (var fileStream = File.Open(rutaCompleta, FileMode.Open, FileAccess.Read))
        {
            IEnumerable<ProductoProveedorDataRow> filas = await _adapter.ReadDataAsync(fileStream, _mapa);
            var listaDeResultados = new List<ProductoProveedorDataRow>();
            int contador = 0;

            foreach (var fila in filas)
            {
                listaDeResultados.Add(fila);
                contador++;

                Console.WriteLine($"Fila procesada: {contador} | " +
                          $"Codigo Externo: {fila.CodigoBarraExterno} | " +
                          $"Nombre: {fila.NombreSugerido} | " +
                          $"Precio: {fila.Precio}");

            }

            Console.WriteLine($"Procesamiento de Excel finalizado. Total: {contador}");
        }
    }
}