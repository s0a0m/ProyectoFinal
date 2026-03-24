using src.Models.Domain;
using Xunit;

namespace Tests.Unit.Domain.CuentaCorriente;

public class NotaCreditoTests
{
    private Factura CrearFactura(decimal total, decimal saldo)
    {
        return new Factura
        {
            IdFactura = 1,
            TotalFacturado = total,
            Saldo = saldo,
            Pagada = saldo == 0,
        };
    }

    private Proveedor CrearProveedor(decimal saldo)
    {
        return new Proveedor { IdProveedor = 1, Saldo = saldo };
    }

    // ===========================
    // Aplicar NC sobre Factura
    // ===========================

    [Fact]
    public void Aplicar_ReduceSaldoFactura()
    {
        var factura = CrearFactura(100000, 100000);
        var proveedor = CrearProveedor(100000);
        var nc = new NotaCredito { Total = 30000 };

        nc.Aplicar(factura, proveedor);

        Assert.Equal(70000, factura.Saldo);
    }

    [Fact]
    public void Aplicar_NCPorTotalDelSaldo_MarcaFacturaPagada()
    {
        var factura = CrearFactura(100000, 100000);
        var proveedor = CrearProveedor(100000);
        var nc = new NotaCredito { Total = 100000 };

        nc.Aplicar(factura, proveedor);

        Assert.Equal(0, factura.Saldo);
        Assert.True(factura.Pagada);
    }

    [Fact]
    public void Aplicar_NCParcial_NoMarcaFacturaPagada()
    {
        var factura = CrearFactura(100000, 100000);
        var proveedor = CrearProveedor(100000);
        var nc = new NotaCredito { Total = 50000 };

        nc.Aplicar(factura, proveedor);

        Assert.False(factura.Pagada);
    }

    // ===========================
    // Aplicar NC sobre Proveedor
    // ===========================

    [Fact]
    public void Aplicar_ReduceSaldoProveedor()
    {
        var factura = CrearFactura(100000, 100000);
        var proveedor = CrearProveedor(200000);
        var nc = new NotaCredito { Total = 30000 };

        nc.Aplicar(factura, proveedor);

        Assert.Equal(170000, proveedor.Saldo);
    }

    // ===========================
    // Multiples NC sobre misma factura
    // ===========================

    [Fact]
    public void Aplicar_MultiplesNC_SaldoSeReduceAcumulativamente()
    {
        var factura = CrearFactura(100000, 100000);
        var proveedor = CrearProveedor(100000);

        var nc1 = new NotaCredito { Total = 20000 };
        var nc2 = new NotaCredito { Total = 30000 };

        nc1.Aplicar(factura, proveedor);
        nc2.Aplicar(factura, proveedor);

        Assert.Equal(50000, factura.Saldo);
        Assert.Equal(50000, proveedor.Saldo);
    }

    [Fact]
    public void Aplicar_MultiplesNC_HastaLlegarACero()
    {
        var factura = CrearFactura(100000, 100000);
        var proveedor = CrearProveedor(100000);

        var nc1 = new NotaCredito { Total = 60000 };
        var nc2 = new NotaCredito { Total = 40000 };

        nc1.Aplicar(factura, proveedor);
        nc2.Aplicar(factura, proveedor);

        Assert.Equal(0, factura.Saldo);
        Assert.True(factura.Pagada);
        Assert.Equal(0, proveedor.Saldo);
    }

    // ===========================
    // NC sobre factura parcialmente pagada
    // ===========================

    [Fact]
    public void Aplicar_SobreFacturaParcialmentePagada_ReduceSaldoRestante()
    {
        // Factura de 100k, ya se pagaron 40k, saldo 60k
        var factura = CrearFactura(100000, 60000);
        var proveedor = CrearProveedor(60000);
        var nc = new NotaCredito { Total = 20000 };

        nc.Aplicar(factura, proveedor);

        Assert.Equal(40000, factura.Saldo);
        Assert.Equal(40000, proveedor.Saldo);
    }
}
