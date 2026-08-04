namespace src.Presentation.ViewModels.EstanteVM;

public class EstanteListItemViewModel
{
    public int     IdEstante      { get; set; }
    public string  NumeroEstante  { get; set; } = string.Empty;
    public bool    Activo         { get; set; }
    public bool    TieneEspacio   { get; set; }
    public string? Observaciones  { get; set; }
    public int     IdDeposito     { get; set; }
    public string  NombreDeposito { get; set; } = string.Empty;
    public int     TotalFilas     { get; set; }
}