using src.Models.Domain;
using Xunit;

namespace Tests.Unit.Domain.CuentaCorriente;

public class FacturaValidacionesTests
{
    // ===========================
    // PuedeEmitirNC
    // ===========================

    [Fact]
    public void PuedeEmitirNC_ConSaldoPendiente_RetornaTrue()
    {
        var factura = new Factura
        {
            TotalFacturado = 100000,
            Saldo = 100000,
            Pagada = false,
        };

        Assert.True(factura.PuedeEmitirNC);
    }

    [Fact]
    public void PuedeEmitirNC_ConSaldoParcial_RetornaTrue()
    {
        var factura = new Factura
        {
            TotalFacturado = 100000,
            Saldo = 30000,
            Pagada = false,
        };

        Assert.True(factura.PuedeEmitirNC);
    }

    [Fact]
    public void PuedeEmitirNC_FacturaPagada_RetornaFalse()
    {
        var factura = new Factura
        {
            TotalFacturado = 100000,
            Saldo = 0,
            Pagada = true,
        };

        Assert.False(factura.PuedeEmitirNC);
    }

    [Fact]
    public void PuedeEmitirNC_SaldoCeroNoPagada_RetornaFalse()
    {
        var factura = new Factura
        {
            TotalFacturado = 100000,
            Saldo = 0,
            Pagada = false,
        };

        Assert.False(factura.PuedeEmitirNC);
    }

    // ===========================
    // PuedeEmitirND
    // ===========================

    [Fact]
    public void PuedeEmitirND_FacturaPendiente_RetornaTrue()
    {
        var factura = new Factura
        {
            TotalFacturado = 100000,
            Saldo = 100000,
            Pagada = false,
        };

        Assert.True(factura.PuedeEmitirND);
    }

    [Fact]
    public void PuedeEmitirND_FacturaPagada_RetornaTrue()
    {
        var factura = new Factura
        {
            TotalFacturado = 100000,
            Saldo = 0,
            Pagada = true,
        };

        Assert.True(factura.PuedeEmitirND);
    }

    // ===========================
    // PuedeRecibirPago
    // ===========================

    [Fact]
    public void PuedeRecibirPago_ConSaldo_RetornaTrue()
    {
        var factura = new Factura
        {
            TotalFacturado = 100000,
            Saldo = 50000,
            Pagada = false,
        };

        Assert.True(factura.PuedeRecibirPago);
    }

    [Fact]
    public void PuedeRecibirPago_SinSaldo_RetornaFalse()
    {
        var factura = new Factura
        {
            TotalFacturado = 100000,
            Saldo = 0,
            Pagada = true,
        };

        Assert.False(factura.PuedeRecibirPago);
    }

    // ===========================
    // PuedeEditarse
    // ===========================

    [Fact]
    public void PuedeEditarse_SinPagosAplicados_RetornaTrue()
    {
        var factura = new Factura { TotalFacturado = 100000, Saldo = 100000 };

        Assert.True(factura.PuedeEditarse);
    }

    [Fact]
    public void PuedeEditarse_ConPagoParcial_RetornaFalse()
    {
        var factura = new Factura { TotalFacturado = 100000, Saldo = 60000 };

        Assert.False(factura.PuedeEditarse);
    }

    [Fact]
    public void PuedeEditarse_Pagada_RetornaFalse()
    {
        var factura = new Factura
        {
            TotalFacturado = 100000,
            Saldo = 0,
            Pagada = true,
        };

        Assert.False(factura.PuedeEditarse);
    }
}
