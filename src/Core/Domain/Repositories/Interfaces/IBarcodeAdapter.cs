using Dom = src.Models.Domain;

namespace src.Repositories.Interfaces;

public interface IBarcodeAdapter
{
    /// <summary>
    /// Valida si una cadena alfanumérica cumple con los requisitos de formato GS1 EAN-13 
    /// </summary>
    bool ValidarFormatoGS1EAN13Async(string codigo);
    Task<string> GenerarBase64CodigoBarraGS1EAN13Async(string codigo, int width = 300, int height = 150, int margin = 2, bool pureBarcode = false, int? fontSize = null);
    Task<string> GenerarBase64CodigoBarraCodigo128Async(
        string paramA, string paramB, string paramC,
        int width = 300,
        int height = 150,
        int margin = 2,
        bool pureBarcode = false,
        int? fontSize = null
    );
}