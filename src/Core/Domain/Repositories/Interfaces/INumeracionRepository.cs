namespace src.Repositories.Interfaces;

public interface INumeracionRepository
{
    Task<string?> ObtenerUltimoNumeroAsync(string prefijo, string tipo);
}
