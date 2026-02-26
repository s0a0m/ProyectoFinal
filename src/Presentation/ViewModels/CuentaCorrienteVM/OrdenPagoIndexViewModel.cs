namespace src.Presentation.ViewModels.CuentaCorrienteVM
{
    public class OrdenPagoIndexVM
    {
        public int IdOrdenPago { get; set; }
        public string Numero { get; set; } = string.Empty;
        public DateTime FechaPago { get; set; }
        public decimal MontoTotal { get; set; }
        public bool Enviada { get; set; }
        public int CantidadFacturasAfectadas { get; set; }
    }
    
    // VM Contenedor para la vista Index completa
    public class ModuloOrdenesPagoVM 
    {
        public int IdProveedor { get; set; }
        public string RazonSocialProveedor { get; set; } = string.Empty;
        public List<OrdenPagoIndexVM> Ordenes { get; set; } = new List<OrdenPagoIndexVM>();
    }
}