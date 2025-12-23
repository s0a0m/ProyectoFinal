namespace src.Models.Domain
{
    public class DetalleFactura
    {
        public int IdDetalleFactura { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioBruto { get; set; }
        public decimal PorcentajeDescuento { get; set; } = 0;
        public decimal PrecioNeto { get; set; }
        public decimal Subtotal => Cantidad * PrecioNeto;
        // public Factura? Factura { get; set; }
        public Producto? Producto { get; set; }
    }
}


    