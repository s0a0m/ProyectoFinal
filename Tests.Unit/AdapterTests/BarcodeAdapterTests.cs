using Xunit;
using System.Threading.Tasks;
using System;
using ZXing;
using src.External;
// Asume que las clases Dom/EF y el namespace src.External son accesibles

namespace src.AdapterTest;

public class BarcodeAdapterTests
{
    // Instancia del adaptador (o mock, si fuera necesario)
    private readonly ZxIngBarcodeAdapter _adapter = new ZxIngBarcodeAdapter();

    // El 'null' es un placeholder; si el adaptador no usa el repo en estos métodos, está bien.
    // Si necesitas inyectar un repositorio, deberás mockearlo.

    // ===================================================================
    // PRUEBA 1: Generación Exitosa de Base64 (Verificación de Integridad)
    // ===================================================================

    [Fact]
    public async Task GenerarBase64_CodigoGS1Valido_RetornaCadenaNoVacia()
    {
        // ARRANGE
        // Código EAN-13 Matemáticamente Válido (Ej: Coca-Cola 779-xxxxxxxxx-x)
        const string codigoValido = "9780201379617";

        // ACT
        string base64Resultado = await _adapter.GenerarBase64CodigoBarraGS1EAN13Async(
            codigoValido,
            width: 150,
            height: 50
        );

        // ASSERT
        // 1. La cadena Base64 no debe ser vacía ni nula
        Assert.False(string.IsNullOrEmpty(base64Resultado));
        // 2. La cadena debe ser lo suficientemente larga (una imagen PNG pequeña)
        Assert.True(base64Resultado.Length > 100, "La cadena Base64 es demasiado corta.");

        // 3. Opcional: Podrías verificar que la cadena Base64 comience con los bytes PNG.
    }

    [Fact]
    public async Task GenerarBase64_Codigo128Valido_RetornaCadenaNoVacia()
    {
        // ARRANGE
        // Código EAN-13 Matemáticamente Válido (Ej: Coca-Cola 779-xxxxxxxxx-x)
        const string paramA = "PepsiNicotra";
        const string paramB = "Lote3";
        const string paramC = "10/2026";

        // ACT
        string base64Resultado = await _adapter.GenerarBase64CodigoBarraCodigo128Async(
            paramA, paramB, paramC,
            width: 150,
            height: 50
        );

        // ASSERT
        // 1. La cadena Base64 no debe ser vacía ni nula
        Assert.False(string.IsNullOrEmpty(base64Resultado));
        // 2. La cadena debe ser lo suficientemente larga (una imagen PNG pequeña)
        Assert.True(base64Resultado.Length > 100, "La cadena Base64 es demasiado corta.");

        // 3. Opcional: Podrías verificar que la cadena Base64 comience con los bytes PNG.
    }


    // ===================================================================
    // PRUEBA 2: Validación Fallida (Manejo de Excepción)
    // ===================================================================

    [Fact]
    public async Task GenerarBase64_CodigoGS1Invalido_LanzaArgumentException()
    {
        // ARRANGE
        // Código EAN-13 Matemáticamente INVÁLIDO (El dígito de control '9' es incorrecto)
        const string codigoInvalido = "7791234567899";

        // ACT & ASSERT
        // Se espera que la validación falle y lance ArgumentException
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _adapter.GenerarBase64CodigoBarraGS1EAN13Async(codigoInvalido)
        );
    }

    // ===================================================================
    // PRUEBA 3: Validación por Longitud
    // ===================================================================

    [Fact]
    public async Task GenerarBase64_CodigoConLongitudIncorrecta_LanzaArgumentException()
    {
        // ARRANGE
        const string codigoCorto = "12345";

        // ACT & ASSERT
        // Se espera que falle debido a que la validación interna (codigo.Length != 13) falle.
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _adapter.GenerarBase64CodigoBarraGS1EAN13Async(codigoCorto)
        );
    }
}