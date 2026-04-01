using src.Core.Services.Interfaces;
using src.Repositories.Interfaces;

namespace src.Core.Services.Implementations;

public class NumeracionService : INumeracionService
{
    private readonly INumeracionRepository _numeracionRepository;

    public NumeracionService(INumeracionRepository numeracionRepository)
    {
        _numeracionRepository = numeracionRepository;
    }

    public async Task<string> GenerarNumeroAsync(string prefijo, string tipo)
    {
        var ultimo = await _numeracionRepository.ObtenerUltimoNumeroAsync(prefijo, tipo);

        if (ultimo == null)
            return $"{prefijo}00000001";

        var parteNumerica = ultimo.Replace(prefijo, "");
        if (int.TryParse(parteNumerica, out int numero))
            return $"{prefijo}{(numero + 1):D8}";

        return $"{prefijo}00000001";
    }
}
