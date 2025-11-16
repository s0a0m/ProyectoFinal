namespace src.Models.Domain;

public class Grupo
{
    public int IdGrupo { get; init; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; }

    public Grupo(int idGrupo, string nombre, string descripcion)
    {
        IdGrupo = idGrupo;
        Nombre = nombre;
        Descripcion = descripcion;
    }
    public Grupo() { }
}