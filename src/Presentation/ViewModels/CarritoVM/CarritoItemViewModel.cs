namespace src.Presentation.ViewModels.CarritoVM
{
    // Este objeto es el que viaja dentro de la Sesión
    public class CarritoItemViewModel
    {
        public short IdProducto { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public string CodigoSku { get; set; } = string.Empty;
        
        public short IdProveedor { get; set; }
        public string NombreProveedor { get; set; } = string.Empty;
        
        public decimal PrecioUnitario { get; set; } // Precio pactado al momento de agregar
        public int Cantidad { get; set; }
        
        public decimal Subtotal => PrecioUnitario * Cantidad;
    }
}