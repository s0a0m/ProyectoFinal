using src.Models.Common;
namespace src.Contracts;

public class ProductoProveedorDataRow
{
    public string CodigoBarraExterno { get; set; } = string.Empty;
    public string NombreSugerido { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public int StockActual { get; set; }
}