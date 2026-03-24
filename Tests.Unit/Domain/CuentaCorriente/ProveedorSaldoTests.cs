using src.Models.Domain;
using Xunit;

namespace Tests.Unit.Domain.CuentaCorriente;

public class ProveedorSaldoTests
{
    // ===========================
    // ReducirSaldo
    // ===========================

    [Fact]
    public void ReducirSaldo_MontoPositivo_ReduceCorrectamente()
    {
        var proveedor = new Proveedor { Saldo = 100000 };

        proveedor.ReducirSaldo(30000);

        Assert.Equal(70000, proveedor.Saldo);
    }

    [Fact]
    public void ReducirSaldo_MontoIgualAlSaldo_QuedaEnCero()
    {
        var proveedor = new Proveedor { Saldo = 50000 };

        proveedor.ReducirSaldo(50000);

        Assert.Equal(0, proveedor.Saldo);
    }

    [Fact]
    public void ReducirSaldo_MontoMayorAlSaldo_SaldoNegativo()
    {
        var proveedor = new Proveedor { Saldo = 30000 };

        proveedor.ReducirSaldo(50000);

        Assert.Equal(-20000, proveedor.Saldo);
    }

    [Fact]
    public void ReducirSaldo_MultiplesReducciones_Acumulativo()
    {
        var proveedor = new Proveedor { Saldo = 100000 };

        proveedor.ReducirSaldo(20000);
        proveedor.ReducirSaldo(30000);
        proveedor.ReducirSaldo(10000);

        Assert.Equal(40000, proveedor.Saldo);
    }

    // ===========================
    // AumentarSaldo
    // ===========================

    [Fact]
    public void AumentarSaldo_MontoPositivo_AumentaCorrectamente()
    {
        var proveedor = new Proveedor { Saldo = 100000 };

        proveedor.AumentarSaldo(20000);

        Assert.Equal(120000, proveedor.Saldo);
    }

    [Fact]
    public void AumentarSaldo_DesdeCero_QuedaPositivo()
    {
        var proveedor = new Proveedor { Saldo = 0 };

        proveedor.AumentarSaldo(50000);

        Assert.Equal(50000, proveedor.Saldo);
    }

    [Fact]
    public void AumentarSaldo_DesdeNegativo_ReduceDeuda()
    {
        var proveedor = new Proveedor { Saldo = -20000 };

        proveedor.AumentarSaldo(50000);

        Assert.Equal(30000, proveedor.Saldo);
    }

    // ===========================
    // Combinados
    // ===========================

    [Fact]
    public void ReducirYAumentar_SaldoQuedaCorrecto()
    {
        var proveedor = new Proveedor { Saldo = 100000 };

        proveedor.AumentarSaldo(20000); // ND: 120k
        proveedor.ReducirSaldo(50000); // NC: 70k
        proveedor.ReducirSaldo(30000); // Pago: 40k

        Assert.Equal(40000, proveedor.Saldo);
    }
}
