using src.Models.Domain;
using Xunit;

namespace Tests.Unit.Domain.CuentaCorriente;

public class NotaDebitoTests
{
    private Factura CrearFactura(decimal total, decimal saldo, bool pagada = false)
    {
        return new Factura
        {
            IdFactura = 1,
            TotalFacturado = total,
            Saldo = saldo,
            Pagada = pagada,
        };
    }

    private Proveedor CrearProveedor(decimal saldo)
    {
        return new Proveedor { IdProveedor = 1, Saldo = saldo };
    }

    // ===========================
    // Aplicar ND sobre Factura
    // ===========================

    [Fact]
    public void Aplicar_AumentaSaldoFactura()
    {
        var factura = CrearFactura(100000, 100000);
        var proveedor = CrearProveedor(100000);
        var nd = new NotaDebito { Total = 20000 };

        nd.Aplicar(factura, proveedor);

        Assert.Equal(120000, factura.Saldo);
    }

    [Fact]
    public void Aplicar_AumentaTotalFacturado()
    {
        var factura = CrearFactura(100000, 100000);
        var proveedor = CrearProveedor(100000);
        var nd = new NotaDebito { Total = 20000 };

        nd.Aplicar(factura, proveedor);

        Assert.Equal(120000, factura.TotalFacturado);
    }

    [Fact]
    public void Aplicar_MarcaFacturaComoNoPagada()
    {
        var factura = CrearFactura(100000, 100000);
        var proveedor = CrearProveedor(100000);
        var nd = new NotaDebito { Total = 20000 };

        nd.Aplicar(factura, proveedor);

        Assert.False(factura.Pagada);
    }

    // ===========================
    // Aplicar ND sobre Proveedor
    // ===========================

    [Fact]
    public void Aplicar_AumentaSaldoProveedor()
    {
        var factura = CrearFactura(100000, 100000);
        var proveedor = CrearProveedor(200000);
        var nd = new NotaDebito { Total = 30000 };

        nd.Aplicar(factura, proveedor);

        Assert.Equal(230000, proveedor.Saldo);
    }

    // ===========================
    // ND sobre factura pagada (reabre deuda)
    // ===========================

    [Fact]
    public void Aplicar_SobreFacturaPagada_ReabreDeuda()
    {
        var factura = CrearFactura(100000, 0, pagada: true);
        var proveedor = CrearProveedor(0);
        var nd = new NotaDebito { Total = 15000 };

        nd.Aplicar(factura, proveedor);

        Assert.Equal(15000, factura.Saldo);
        Assert.Equal(115000, factura.TotalFacturado);
        Assert.False(factura.Pagada);
    }

    [Fact]
    public void Aplicar_SobreFacturaPagada_AumentaSaldoProveedor()
    {
        var factura = CrearFactura(100000, 0, pagada: true);
        var proveedor = CrearProveedor(0);
        var nd = new NotaDebito { Total = 15000 };

        nd.Aplicar(factura, proveedor);

        Assert.Equal(15000, proveedor.Saldo);
    }

    // ===========================
    // Multiples ND
    // ===========================

    [Fact]
    public void Aplicar_MultiplesND_SaldoSeAcumula()
    {
        var factura = CrearFactura(100000, 100000);
        var proveedor = CrearProveedor(100000);

        var nd1 = new NotaDebito { Total = 10000 };
        var nd2 = new NotaDebito { Total = 5000 };

        nd1.Aplicar(factura, proveedor);
        nd2.Aplicar(factura, proveedor);

        Assert.Equal(115000, factura.Saldo);
        Assert.Equal(115000, factura.TotalFacturado);
        Assert.Equal(115000, proveedor.Saldo);
    }

    // ===========================
    // ND + NC combinados
    // ===========================

    [Fact]
    public void Aplicar_NDLuegoNC_SaldoQuedaCorrecto()
    {
        var factura = CrearFactura(100000, 100000);
        var proveedor = CrearProveedor(100000);

        // ND aumenta 20k
        var nd = new NotaDebito { Total = 20000 };
        nd.Aplicar(factura, proveedor);

        // NC reduce 50k
        var nc = new NotaCredito { Total = 50000 };
        nc.Aplicar(factura, proveedor);

        Assert.Equal(70000, factura.Saldo); // 120k - 50k
        Assert.Equal(120000, factura.TotalFacturado); // 100k + 20k
        Assert.Equal(70000, proveedor.Saldo); // 120k - 50k
    }
}
