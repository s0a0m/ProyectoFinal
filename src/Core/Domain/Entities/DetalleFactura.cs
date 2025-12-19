namespace src.Models.Domain
{
    public class DetalleFactura
    {
        public int IdDetalleFactura { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal => Cantidad * PrecioUnitario;
        // public Factura? Factura { get; set; }
        public Producto? Producto { get; set; }
    }
}