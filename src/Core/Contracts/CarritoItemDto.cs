namespace src.Core.Contracts;

public class CarritoItemDto
{
    public short IdProducto { get; set; }
    public string NombreProducto { get; set; } = string.Empty;
    public List<string> CodigosExternos { get; set; } = new();
    public short IdProveedor { get; set; }
    public string NombreProveedor { get; set; } = string.Empty;
    public decimal PrecioUnitario { get; set; }
    public int Cantidad { get; set; }

    public decimal Subtotal => PrecioUnitario * Cantidad;
}
