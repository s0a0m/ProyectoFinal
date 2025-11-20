using Dom = src.Models.Domain;

namespace src.Repositories.Interfaces;

public interface IBarcodeAdapter
{
    string FormatearCodigo128(string paramA, string paramB, string paramC);
    bool ValidarFormatoCodigo128Async(string codigo);
    Task<string> GenerarBase64CodigoBarraCodigo128Async(
        string paramA, string paramB, string paramC,
        int width = 300,
        int height = 150,
        int margin = 2,
        bool pureBarcode = false,
        int? fontSize = null
    );
    bool ValidarFormatoGS1EAN13Async(string codigo);
    string FormatearEAN13(string codigo);
    Task<string> GenerarBase64CodigoBarraGS1EAN13Async(string codigo, int width = 300, int height = 150, int margin = 2, bool pureBarcode = false, int? fontSize = null);
    // cambiar nombre luego
    bool ValidarCodigoExtra(string codigo);
}