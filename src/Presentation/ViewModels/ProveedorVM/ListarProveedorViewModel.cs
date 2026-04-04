namespace src.Presentation.ViewModels.ProveedorVM
{
    public class ListarProveedorViewModel
    {
        public int IdProveedor { get; set; }
        public string RazonSocial { get; set; } = string.Empty;
        public string Cuit { get; set; } = string.Empty;
        public string PersonaResponsable { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public decimal SaldoActual { get; set; }
        public bool Activo { get; set; }
    }
}
