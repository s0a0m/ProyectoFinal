namespace src.Presentation.ViewModels.CuentaCorrienteVM
{
    public class OrdenPagoDetalleModalVM
    {
        // Cabecera
        public int IdOrdenPago { get; set; }
        public string NumeroOrden { get; set; } = string.Empty;
        public DateTime FechaPago { get; set; }
        public decimal MontoTotal { get; set; }
        public string Estado { get; set; } = string.Empty; // "Enviada" o "Borrador"

        // Detalles (Qué facturas pagó)
        public List<DetallePagoItemVM> Items { get; set; } = new List<DetallePagoItemVM>();
    }

    public class DetallePagoItemVM
    {
        public string NumeroFactura { get; set; } = string.Empty;
        public DateTime FechaEmisionFactura { get; set; }
        public decimal ImportePagado { get; set; }
    }
}