using src.Models.Domain;
using Xunit;

namespace Tests.Unit.Domain.CuentaCorriente;

public class FacturaOperacionesTests
{
    // ===========================
    // AplicarPago
    // ===========================

    [Fact]
    public void AplicarPago_PagoTotal_SaldoQuedaEnCero()
    {
        var factura = new Factura { TotalFacturado = 100000, Saldo = 100000 };

        factura.AplicarPago(100000);

        Assert.Equal(0, factura.Saldo);
    }

    [Fact]
    public void AplicarPago_PagoTotal_MarcaComoPagada()
    {
        var factura = new Factura { TotalFacturado = 100000, Saldo = 100000 };

        factura.AplicarPago(100000);

        Assert.True(factura.Pagada);
    }

    [Fact]
    public void AplicarPago_PagoTotal_AsignaFechaPago()
    {
        var factura = new Factura { TotalFacturado = 100000, Saldo = 100000 };
        var antes = DateTime.UtcNow;

        factura.AplicarPago(100000);

        Assert.True(factura.FechaPago >= antes);
    }

    [Fact]
    public void AplicarPago_PagoParcial_ReduceSaldo()
    {
        var factura = new Factura { TotalFacturado = 100000, Saldo = 100000 };

        factura.AplicarPago(40000);

        Assert.Equal(60000, factura.Saldo);
    }

    [Fact]
    public void AplicarPago_PagoParcial_NoMarcaComoPagada()
    {
        var factura = new Factura { TotalFacturado = 100000, Saldo = 100000 };

        factura.AplicarPago(40000);

        Assert.False(factura.Pagada);
    }

    [Fact]
    public void AplicarPago_MultipesPagosParciales_SaldoCorreto()
    {
        var factura = new Factura { TotalFacturado = 100000, Saldo = 100000 };

        factura.AplicarPago(30000);
        factura.AplicarPago(30000);
        factura.AplicarPago(40000);

        Assert.Equal(0, factura.Saldo);
        Assert.True(factura.Pagada);
    }

    // ===========================
    // RecalcularSaldo
    // ===========================

    [Fact]
    public void RecalcularSaldo_SinPagosPrevios_SaldoIgualAlNuevoTotal()
    {
        var factura = new Factura { TotalFacturado = 100000, Saldo = 100000 };

        factura.RecalcularSaldo(120000);

        Assert.Equal(120000, factura.TotalFacturado);
        Assert.Equal(120000, factura.Saldo);
    }

    [Fact]
    public void RecalcularSaldo_ConPagosPrevios_RespetaMontoPagado()
    {
        // Factura de 100k, se pagaron 40k, saldo es 60k
        var factura = new Factura { TotalFacturado = 100000, Saldo = 60000 };

        // Editar total a 120k, los 40k pagados se respetan
        factura.RecalcularSaldo(120000);

        Assert.Equal(120000, factura.TotalFacturado);
        Assert.Equal(80000, factura.Saldo); // 120k - 40k pagados
    }

    [Fact]
    public void RecalcularSaldo_NuevoTotalMenorAlPagado_SaldoNegativo()
    {
        // Factura de 100k, se pagaron 80k, saldo es 20k
        var factura = new Factura { TotalFacturado = 100000, Saldo = 20000 };

        // Editar total a 50k, pero ya se pagaron 80k
        factura.RecalcularSaldo(50000);

        Assert.Equal(50000, factura.TotalFacturado);
        Assert.Equal(-30000, factura.Saldo); // 50k - 80k = negativo (se detecta como error)
    }

    [Fact]
    public void RecalcularSaldo_NuevoTotalIgualAlPagado_MarcaPagada()
    {
        // Factura de 100k, se pagaron 60k
        var factura = new Factura { TotalFacturado = 100000, Saldo = 40000 };

        // Editar total a 60k, justo lo que se pagó
        factura.RecalcularSaldo(60000);

        Assert.Equal(0, factura.Saldo);
        Assert.True(factura.Pagada);
    }
}
