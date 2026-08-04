namespace src.Presentation.ViewModels.DepositoVM;

public class DepositoListItemViewModel
{
    public int    IdDeposito      { get; set; }
    public string Nombre          { get; set; } = string.Empty;
    public string DireccionResumen{ get; set; } = string.Empty;
    public bool   Activo          { get; set; }
    public int    TotalEstantes   { get; set; }
    public int    TotalFilas      { get; set; }


    public string Calle        { get; set; } = string.Empty;
    public short  Numero       { get; set; }
    public short? Piso         { get; set; }
    public string Comentario   { get; set; } = string.Empty;
    public short  IdProvincia  { get; set; }

}