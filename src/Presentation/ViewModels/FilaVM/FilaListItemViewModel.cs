namespace src.Presentation.ViewModels.FilaVM;

public class FilaListItemViewModel
{
    public int     IdFila         { get; set; }
    public string  NFila          { get; set; } = string.Empty;
    public bool    Activo         { get; set; }
    public bool    TieneEspacio   { get; set; }
    public string? Observaciones  { get; set; }
    public int     IdEstante      { get; set; }
    public string  NumeroEstante  { get; set; } = string.Empty;
    public string  NombreDeposito { get; set; } = string.Empty;
    public int     IdDeposito     { get; set; }
}