namespace src.Core.Services.Interfaces;

public interface INumeracionService
{
    Task<string> GenerarNumeroAsync(string prefijo, string tipo);
}
