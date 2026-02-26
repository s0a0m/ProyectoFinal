namespace src.Presentation.ViewModels.CarritoVM
{
    public class CarritoItemViewModel
    {
        public short IdProducto { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        
        // CAMBIO: Usaremos el primer código externo disponible o "-"
        public List<string> CodigosExternos { get; set; } = new();
        
        public short IdProveedor { get; set; }
        public string NombreProveedor { get; set; } = string.Empty;
        
        public decimal PrecioUnitario { get; set; }
        public int Cantidad { get; set; }
        
        public decimal Subtotal => PrecioUnitario * Cantidad;
    }
}