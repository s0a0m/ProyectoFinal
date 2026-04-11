namespace src.Core.Contracts;

public class CarritoItemDto
{
    public int IdProducto { get; set; }
    public string NombreProducto { get; set; } = string.Empty;
    public string CodigoProducto { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public short IdProveedor { get; set; }
    public string NombreProveedor { get; set; } = string.Empty;

    public decimal Subtotal => Cantidad * PrecioUnitario;
}
