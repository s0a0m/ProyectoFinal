using src.Models.Domain;
using Xunit;

namespace Tests.Unit.Domain.CuentaCorriente;

/// <summary>
/// Tests que simulan flujos completos de cuenta corriente
/// verificando que los estados y saldos sean consistentes
/// en cada paso del proceso.
/// </summary>
public class FlujoCompletoTests
{
    private Factura CrearFactura(decimal total)
    {
        return new Factura
        {
            IdFactura = 1,
            NumeroFactura = "FAC-001",
            TotalFacturado = total,
            Saldo = total,
            Pagada = false,
        };
    }

    private Proveedor CrearProveedor(decimal saldoInicial)
    {
        return new Proveedor { IdProveedor = 1, Saldo = saldoInicial };
    }

    // ===========================
    // Flujo: Factura → Pago total
    // ===========================

    [Fact]
    public void Flujo_CrearFactura_PagarTotal_SaldosCuadran()
    {
        var proveedor = CrearProveedor(0);
        var factura = CrearFactura(100000);
        proveedor.AumentarSaldo(100000); // Simula que crear factura sube saldo

        // Verificar estado inicial
        Assert.True(factura.PuedeRecibirPago);
        Assert.True(factura.PuedeEmitirNC);
        Assert.True(factura.PuedeEditarse);

        // Pagar
        factura.AplicarPago(100000);
        proveedor.ReducirSaldo(100000);

        // Verificar estado final
        Assert.Equal(0, factura.Saldo);
        Assert.True(factura.Pagada);
        Assert.Equal(0, proveedor.Saldo);
        Assert.False(factura.PuedeRecibirPago);
        Assert.False(factura.PuedeEmitirNC);
    }

    // ===========================
    // Flujo: Factura → NC → Pago del resto
    // ===========================

    [Fact]
    public void Flujo_Factura_NC_PagoResto_SaldosCuadran()
    {
        var proveedor = CrearProveedor(100000);
        var factura = CrearFactura(100000);

        // Paso 1: NC por 30k
        Assert.True(factura.PuedeEmitirNC);
        var nc = new NotaCredito { Total = 30000 };
        nc.Aplicar(factura, proveedor);

        Assert.Equal(70000, factura.Saldo);
        Assert.Equal(70000, proveedor.Saldo);
        Assert.False(factura.Pagada);
        Assert.True(factura.PuedeRecibirPago);
        Assert.False(factura.PuedeEditarse); // TotalFacturado != Saldo

        // Paso 2: Pagar los 70k restantes
        factura.AplicarPago(70000);
        proveedor.ReducirSaldo(70000);

        Assert.Equal(0, factura.Saldo);
        Assert.True(factura.Pagada);
        Assert.Equal(0, proveedor.Saldo);
    }

    // ===========================
    // Flujo: Factura → Pago total → ND (reabre) → Pago
    // ===========================

    [Fact]
    public void Flujo_Factura_PagoTotal_ND_PagoNuevo_SaldosCuadran()
    {
        var proveedor = CrearProveedor(100000);
        var factura = CrearFactura(100000);

        // Paso 1: Pagar todo
        factura.AplicarPago(100000);
        proveedor.ReducirSaldo(100000);

        Assert.True(factura.Pagada);
        Assert.Equal(0, proveedor.Saldo);
        Assert.False(factura.PuedeRecibirPago);
        Assert.False(factura.PuedeEmitirNC);

        // Paso 2: ND reabre la deuda
        Assert.True(factura.PuedeEmitirND);
        var nd = new NotaDebito { Total = 15000 };
        nd.Aplicar(factura, proveedor);

        Assert.Equal(15000, factura.Saldo);
        Assert.False(factura.Pagada);
        Assert.Equal(15000, proveedor.Saldo);
        Assert.True(factura.PuedeRecibirPago);
        Assert.True(factura.PuedeEmitirNC);

        // Paso 3: Pagar lo nuevo
        factura.AplicarPago(15000);
        proveedor.ReducirSaldo(15000);

        Assert.Equal(0, factura.Saldo);
        Assert.True(factura.Pagada);
        Assert.Equal(0, proveedor.Saldo);
    }

    // ===========================
    // Flujo: Factura → ND → NC → Pagos parciales
    // ===========================

    [Fact]
    public void Flujo_Factura_ND_NC_PagosParciales_SaldosCuadran()
    {
        var proveedor = CrearProveedor(100000);
        var factura = CrearFactura(100000);

        // Paso 1: ND aumenta 20k
        var nd = new NotaDebito { Total = 20000 };
        nd.Aplicar(factura, proveedor);

        Assert.Equal(120000, factura.Saldo);
        Assert.Equal(120000, factura.TotalFacturado);
        Assert.Equal(120000, proveedor.Saldo);

        // Paso 2: NC reduce 40k
        var nc = new NotaCredito { Total = 40000 };
        nc.Aplicar(factura, proveedor);

        Assert.Equal(80000, factura.Saldo);
        Assert.Equal(80000, proveedor.Saldo);

        // Paso 3: Pago parcial 50k
        factura.AplicarPago(50000);
        proveedor.ReducirSaldo(50000);

        Assert.Equal(30000, factura.Saldo);
        Assert.Equal(30000, proveedor.Saldo);
        Assert.False(factura.Pagada);

        // Paso 4: Pago final 30k
        factura.AplicarPago(30000);
        proveedor.ReducirSaldo(30000);

        Assert.Equal(0, factura.Saldo);
        Assert.True(factura.Pagada);
        Assert.Equal(0, proveedor.Saldo);
    }

    // ===========================
    // Flujo: Orden borrador → NC cambia saldo → Confirmar falla
    // ===========================

    [Fact]
    public void Flujo_OrdenBorrador_NCReduceSaldo_OrdenExcedeSaldo()
    {
        var proveedor = CrearProveedor(100000);
        var factura = CrearFactura(100000);

        // Paso 1: Crear orden por 100k (borrador, no impacta saldo)
        decimal montoOrden = 100000;
        Assert.True(factura.PuedeRecibirPago);

        // Paso 2: NC reduce saldo a 30k
        var nc = new NotaCredito { Total = 70000 };
        nc.Aplicar(factura, proveedor);

        Assert.Equal(30000, factura.Saldo);

        // Paso 3: Al intentar confirmar, el monto excede el saldo actual
        Assert.True(montoOrden > factura.Saldo);
        // El servicio rechazaría esta confirmación
    }

    // ===========================
    // Flujo: Multiples facturas con un proveedor
    // ===========================

    [Fact]
    public void Flujo_MultiplesFacturas_SaldoProveedorAcumulado()
    {
        var proveedor = CrearProveedor(0);

        // Factura 1: 100k
        var factura1 = CrearFactura(100000);
        proveedor.AumentarSaldo(100000);

        // Factura 2: 50k
        var factura2 = new Factura
        {
            IdFactura = 2,
            TotalFacturado = 50000,
            Saldo = 50000,
            Pagada = false,
        };
        proveedor.AumentarSaldo(50000);

        Assert.Equal(150000, proveedor.Saldo);

        // NC sobre factura 1 por 20k
        var nc = new NotaCredito { Total = 20000 };
        nc.Aplicar(factura1, proveedor);

        Assert.Equal(80000, factura1.Saldo);
        Assert.Equal(50000, factura2.Saldo);
        Assert.Equal(130000, proveedor.Saldo);

        // Pagar factura 2 completa
        factura2.AplicarPago(50000);
        proveedor.ReducirSaldo(50000);

        Assert.Equal(80000, factura1.Saldo);
        Assert.Equal(0, factura2.Saldo);
        Assert.True(factura2.Pagada);
        Assert.Equal(80000, proveedor.Saldo);
    }

    // ===========================
    // Validación: NC no puede superar saldo
    // ===========================

    [Fact]
    public void Validacion_NCMayorAlSaldo_FacturaNoPuedeEmitir()
    {
        var factura = CrearFactura(100000);

        // Pagar parcialmente
        factura.AplicarPago(80000);

        Assert.Equal(20000, factura.Saldo);
        Assert.True(factura.PuedeEmitirNC);

        // Una NC por 30k excedería el saldo de 20k
        // El servicio valida: comprobante.Total > factura.Saldo
        decimal montoNC = 30000;
        Assert.True(montoNC > factura.Saldo);
    }

    // ===========================
    // Validación: Factura pagada no puede recibir NC ni pago
    // ===========================

    [Fact]
    public void Validacion_FacturaPagada_SoloPermiteND()
    {
        var factura = CrearFactura(100000);
        factura.AplicarPago(100000);

        Assert.True(factura.Pagada);
        Assert.False(factura.PuedeEmitirNC);
        Assert.False(factura.PuedeRecibirPago);
        Assert.True(factura.PuedeEmitirND);
        Assert.False(factura.PuedeEditarse);
    }
}
