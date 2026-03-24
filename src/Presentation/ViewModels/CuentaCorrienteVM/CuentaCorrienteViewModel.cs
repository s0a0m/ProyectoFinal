namespace src.Presentation.ViewModels.CuentaCorrienteVM
{
    public class CuentaCorrienteVM
    {
        public int IdProveedor { get; set; }
        public string RazonSocial { get; set; } = string.Empty;
        public string PersonaResponsable { get; set; } = string.Empty;
        public string Cuit { get; set; } = string.Empty;

        // Tarjetas resumen
        public decimal TotalFacturado { get; set; }
        public decimal TotalPagado { get; set; }
        public decimal TotalNC { get; set; }
        public decimal TotalND { get; set; }
        public decimal SaldoTotal { get; set; }

        // Facturas agrupadas con sus documentos
        public List<FacturaCCVM> Facturas { get; set; } = new();

        // Motivos para el modal de crear NC/ND
        public List<MotivoCCVM> MotivosNC { get; set; } = new();
        public List<MotivoCCVM> MotivosND { get; set; } = new();
    }

    public class MotivoCCVM
    {
        public short IdMotivo { get; set; }
        public string Descripcion { get; set; } = string.Empty;
    }

    public class FacturaCCVM
    {
        public int IdFactura { get; set; }
        public string NumeroFactura { get; set; } = string.Empty;
        public DateTime FechaEmision { get; set; }
        public decimal TotalFacturado { get; set; }
        public decimal Saldo { get; set; }
        public bool Pagada { get; set; }
        public bool PuedeEditarse { get; set; }
        public string Estado =>
            Pagada ? "Pagada"
            : Saldo < TotalFacturado ? "Parcial"
            : "Pendiente";

        public List<NotaCCVM> Notas { get; set; } = new();
        public List<PagoCCVM> Pagos { get; set; } = new();
    }

    public class NotaCCVM
    {
        public int IdComprobante { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string Motivo { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public DateTime FechaEmision { get; set; }
    }

    public class PagoCCVM
    {
        public int IdOrdenPago { get; set; }
        public string Numero { get; set; } = string.Empty;
        public DateTime? FechaPago { get; set; }
        public decimal MontoAplicado { get; set; }
        public bool Enviada { get; set; }
    }
}
