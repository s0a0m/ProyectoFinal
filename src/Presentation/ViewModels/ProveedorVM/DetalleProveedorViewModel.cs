namespace src.Presentation.ViewModels.ProveedorVM
{
    public class DetalleProveedorViewModel
    {
        public int IdProveedor { get; set; }
        public string RazonSocial { get; set; } = string.Empty;
        public string Cuit { get; set; } = string.Empty;
        public decimal SaldoActual { get; set; }
        public decimal SaldoInicial { get; set; }
        public bool Estado { get; set; }
        public string PersonaResponsable { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Provincia { get; set; } = string.Empty;
        public string Calle { get; set; } = string.Empty;
        public int Altura { get; set; }
        public int? Piso { get; set; }
        public string Comentario { get; set; } = string.Empty;
        public CondicionPagoDetalleVM CondicionPago { get; set; } = new();
    }

    public class CondicionPagoDetalleVM
    {
        public string Nombre { get; set; } = string.Empty;
        public string IntervaloDias { get; set; } = string.Empty;
        public int? NumeroCuotas { get; set; }
        public decimal? InteresPorcentual { get; set; }
    }
}
