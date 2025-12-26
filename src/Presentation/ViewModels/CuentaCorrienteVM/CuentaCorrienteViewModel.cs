namespace src.Presentation.ViewModels.CuentaCorrienteVM
{
    public class CuentaCorrienteVM
    {
        // Cabecera
        public int IdProveedor { get; set; }
        public string RazonSocial { get; set; } = string.Empty;
        public string PersonaResponsable { get; set; } = string.Empty;
        public string Cuit { get; set; } = string.Empty;
        public decimal SaldoTotal { get; set; }

        // Lista unificada
        public List<MovimientoCuentaCorrienteItemVM> Movimientos { get; set; } = new List<MovimientoCuentaCorrienteItemVM>();
    }

    public class MovimientoCuentaCorrienteItemVM
    {
        public int IdReferencia { get; set; } // IdFactura, IdOrden, o IdComprobante
        public string TipoMovimiento { get; set; } = string.Empty; // "Factura", "Orden Pago", "Nota Crédito", etc.
        public DateTime Fecha { get; set; }
        public string NumeroComprobante { get; set; } = string.Empty;
        public decimal MontoTotal { get; set; }
        public decimal? SaldoPendiente { get; set; } // Solo para facturas
        
        // Lógica de UI pre-calculada
        public bool EsFactura { get; set; }
        public bool EsOrdenPago { get; set; }
        public bool EsComprobante { get; set; } // Notas
        
        // Permisos de acción según tus reglas de negocio
        public bool PuedeEditar { get; set; } 
        public bool PuedeEliminar { get; set; }
        public bool PuedeVerRelaciones { get; set; } // Para el GetByFacturaId
    }
}