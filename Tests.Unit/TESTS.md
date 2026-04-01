# Tests de Cuenta Corriente — Documentación

## Resumen

| Archivo | Tests | Qué verifica |
|---------|-------|-------------|
| FacturaValidacionesTests | 10 | Reglas de qué operaciones permite cada estado de factura |
| FacturaOperacionesTests | 10 | Cálculos de saldo al pagar y al editar factura |
| NotaCreditoTests | 7 | NC reduce saldo de factura y proveedor correctamente |
| NotaDebitoTests | 8 | ND aumenta saldo y puede reabrir facturas pagadas |
| ProveedorSaldoTests | 8 | Operaciones de saldo del proveedor |
| FlujoCompletoTests | 7 | Flujos reales de cuenta corriente de principio a fin |

**Total: 56 tests**

---

## FacturaValidacionesTests

Verifica las propiedades que determinan qué acciones están permitidas según el estado actual de la factura. Estas propiedades son consultadas por los servicios antes de ejecutar cualquier operación.

| Test | Escenario | Resultado esperado | Por qué importa |
|------|-----------|-------------------|-----------------|
| `PuedeEmitirNC_ConSaldoPendiente_RetornaTrue` | Factura nueva, saldo completo | Puede emitir NC | Caso base: toda factura pendiente acepta NC |
| `PuedeEmitirNC_ConSaldoParcial_RetornaTrue` | Factura con pagos parciales | Puede emitir NC | Se puede acreditar el saldo restante |
| `PuedeEmitirNC_FacturaPagada_RetornaFalse` | Factura saldo=0, pagada | No puede emitir NC | No tiene sentido acreditar algo ya pagado |
| `PuedeEmitirNC_SaldoCeroNoPagada_RetornaFalse` | Saldo=0 pero Pagada=false | No puede emitir NC | Edge case: sin saldo no se puede acreditar |
| `PuedeEmitirND_FacturaPendiente_RetornaTrue` | Factura pendiente | Puede emitir ND | Caso normal: agregar deuda |
| `PuedeEmitirND_FacturaPagada_RetornaTrue` | Factura pagada | Puede emitir ND | Clave: ND reabre deuda en facturas cerradas |
| `PuedeRecibirPago_ConSaldo_RetornaTrue` | Factura con saldo | Puede recibir pago | Caso base de orden de pago |
| `PuedeRecibirPago_SinSaldo_RetornaFalse` | Factura pagada | No puede recibir pago | Evita pagos duplicados |
| `PuedeEditarse_SinPagosAplicados_RetornaTrue` | Saldo == TotalFacturado | Puede editarse | Solo se edita si no se tocó |
| `PuedeEditarse_ConPagoParcial_RetornaFalse` | Saldo < TotalFacturado | No puede editarse | Editar rompería la cadena contable |
| `PuedeEditarse_Pagada_RetornaFalse` | Factura pagada | No puede editarse | Factura cerrada es inmutable |

### Diagrama de estados que cubren estos tests:

```
┌─────────────────────────────────────────────────────────┐
│ Estado              │ NC │ ND │ Pago │ Editar          │
├─────────────────────┼────┼────┼──────┼─────────────────┤
│ Pendiente           │ ✅ │ ✅ │  ✅  │  ✅             │
│ Parcialmente pagada │ ✅ │ ✅ │  ✅  │  ❌             │
│ Pagada              │ ❌ │ ✅ │  ❌  │  ❌             │
└─────────────────────────────────────────────────────────┘
```

---

## FacturaOperacionesTests

Verifica que los métodos que modifican el saldo de la factura calculen correctamente. Un error acá significa plata que aparece o desaparece del sistema.

### AplicarPago

| Test | Escenario | Resultado esperado | Por qué importa |
|------|-----------|-------------------|-----------------|
| `AplicarPago_PagoTotal_SaldoQuedaEnCero` | Pago por el total | Saldo = 0 | Caso base de pago completo |
| `AplicarPago_PagoTotal_MarcaComoPagada` | Pago por el total | Pagada = true | La factura debe cerrarse |
| `AplicarPago_PagoTotal_AsignaFechaPago` | Pago por el total | FechaPago tiene valor | Registro de cuándo se pagó |
| `AplicarPago_PagoParcial_ReduceSaldo` | Pago de 40k sobre 100k | Saldo = 60k | Aritmética correcta |
| `AplicarPago_PagoParcial_NoMarcaComoPagada` | Pago parcial | Pagada = false | No cerrar prematuramente |
| `AplicarPago_MultipesPagosParciales_SaldoCorreto` | 30k + 30k + 40k | Saldo = 0, Pagada | Pagos acumulativos funcionan |

### RecalcularSaldo

| Test | Escenario | Resultado esperado | Por qué importa |
|------|-----------|-------------------|-----------------|
| `RecalcularSaldo_SinPagosPrevios_SaldoIgualAlNuevoTotal` | Editar factura sin pagos | Saldo = nuevo total | Caso simple de edición |
| `RecalcularSaldo_ConPagosPrevios_RespetaMontoPagado` | 40k pagados, editar a 120k | Saldo = 80k | BUG-001: antes se perdían los pagos |
| `RecalcularSaldo_NuevoTotalMenorAlPagado_SaldoNegativo` | 80k pagados, editar a 50k | Saldo = -30k | Detecta inconsistencia en vez de esconderla |
| `RecalcularSaldo_NuevoTotalIgualAlPagado_MarcaPagada` | 60k pagados, editar a 60k | Pagada = true | Cierre correcto por edición |

---

## NotaCreditoTests

Verifica que una Nota de Crédito reduce correctamente el saldo de la factura y del proveedor. Una NC representa que el proveedor nos debe menos (descuento, devolución, error de facturación).

| Test | Escenario | Resultado esperado | Por qué importa |
|------|-----------|-------------------|-----------------|
| `Aplicar_ReduceSaldoFactura` | NC 30k sobre factura 100k | Saldo = 70k | Operación básica de NC |
| `Aplicar_NCPorTotalDelSaldo_MarcaFacturaPagada` | NC por el total | Pagada = true | NC cancela la deuda completamente |
| `Aplicar_NCParcial_NoMarcaFacturaPagada` | NC por menos del saldo | Pagada = false | Queda deuda pendiente |
| `Aplicar_ReduceSaldoProveedor` | NC 30k, proveedor 200k | Proveedor saldo = 170k | NC impacta al proveedor |
| `Aplicar_MultiplesNC_SaldoSeReduceAcumulativamente` | NC 20k + NC 30k | Saldo = 50k | Varias NC sobre misma factura |
| `Aplicar_MultiplesNC_HastaLlegarACero` | NC 60k + NC 40k | Saldo = 0, Pagada | NC acumuladas cancelan la deuda |
| `Aplicar_SobreFacturaParcialmentePagada_ReduceSaldoRestante` | Factura con saldo 60k, NC 20k | Saldo = 40k | NC opera sobre saldo actual, no total |

---

## NotaDebitoTests

Verifica que una Nota de Débito aumenta correctamente el saldo. Una ND representa que el proveedor nos cobra más (intereses, mora, ajuste de precio). Es el único documento que puede reabrir una factura pagada.

| Test | Escenario | Resultado esperado | Por qué importa |
|------|-----------|-------------------|-----------------|
| `Aplicar_AumentaSaldoFactura` | ND 20k sobre factura 100k | Saldo = 120k | Operación básica de ND |
| `Aplicar_AumentaTotalFacturado` | ND 20k | TotalFacturado = 120k | El total también sube |
| `Aplicar_MarcaFacturaComoNoPagada` | ND sobre factura | Pagada = false | Siempre queda pendiente |
| `Aplicar_AumentaSaldoProveedor` | ND 30k, proveedor 200k | Proveedor saldo = 230k | ND impacta al proveedor |
| `Aplicar_SobreFacturaPagada_ReabreDeuda` | Factura pagada, ND 15k | Saldo = 15k, Pagada = false | Clave: ND reabre deuda |
| `Aplicar_SobreFacturaPagada_AumentaSaldoProveedor` | Proveedor saldo=0, ND 15k | Proveedor saldo = 15k | El proveedor vuelve a tener deuda |
| `Aplicar_MultiplesND_SaldoSeAcumula` | ND 10k + ND 5k | Saldo = 115k | Múltiples ND acumulan |
| `Aplicar_NDLuegoNC_SaldoQuedaCorrecto` | ND +20k, NC -50k | Saldo = 70k | Combinación ND + NC |

---

## ProveedorSaldoTests

Verifica que las operaciones de saldo del proveedor sean aritméticas puras sin efectos secundarios inesperados.

| Test | Escenario | Resultado esperado | Por qué importa |
|------|-----------|-------------------|-----------------|
| `ReducirSaldo_MontoPositivo_ReduceCorrectamente` | 100k - 30k | 70k | Operación básica |
| `ReducirSaldo_MontoIgualAlSaldo_QuedaEnCero` | 50k - 50k | 0 | Caso borde |
| `ReducirSaldo_MontoMayorAlSaldo_SaldoNegativo` | 30k - 50k | -20k | Detecta inconsistencia |
| `ReducirSaldo_MultiplesReducciones_Acumulativo` | 100k - 20k - 30k - 10k | 40k | Múltiples operaciones |
| `AumentarSaldo_MontoPositivo_AumentaCorrectamente` | 100k + 20k | 120k | Operación básica |
| `AumentarSaldo_DesdeCero_QuedaPositivo` | 0 + 50k | 50k | Caso base |
| `AumentarSaldo_DesdeNegativo_ReduceDeuda` | -20k + 50k | 30k | Edge case con saldo negativo |
| `ReducirYAumentar_SaldoQuedaCorrecto` | +20k, -50k, -30k | 40k | Mezcla de operaciones |

---

## FlujoCompletoTests

Tests que simulan escenarios reales de cuenta corriente completos, verificando que los saldos cuadren en cada paso. Son los más importantes porque prueban la interacción entre todos los componentes.

| Test | Flujo | Qué verifica |
|------|-------|-------------|
| `Flujo_CrearFactura_PagarTotal_SaldosCuadran` | Factura → Pago total | El camino feliz más simple funciona |
| `Flujo_Factura_NC_PagoResto_SaldosCuadran` | Factura → NC → Pago | NC reduce y después se paga el resto |
| `Flujo_Factura_PagoTotal_ND_PagoNuevo_SaldosCuadran` | Factura → Pago → ND → Pago | ND reabre factura pagada y se vuelve a pagar |
| `Flujo_Factura_ND_NC_PagosParciales_SaldosCuadran` | Factura → ND → NC → Pagos parciales | Combinación completa con múltiples operaciones |
| `Flujo_OrdenBorrador_NCReduceSaldo_OrdenExcedeSaldo` | Orden borrador → NC → Confirmar falla | Detecta que una orden queda obsoleta por NC posterior |
| `Flujo_MultiplesFacturas_SaldoProveedorAcumulado` | 2 facturas → NC → Pago | Saldo del proveedor acumula correctamente |
| `Validacion_FacturaPagada_SoloPermiteND` | Factura pagada | Verifica qué operaciones permite y cuáles no |

### Diagrama del flujo más complejo testeado:

```
Factura $100.000
    │
    ├── ND +$20.000  →  Saldo: $120.000  Proveedor: $120.000
    │
    ├── NC -$40.000  →  Saldo: $80.000   Proveedor: $80.000
    │
    ├── Pago $50.000 →  Saldo: $30.000   Proveedor: $30.000
    │
    └── Pago $30.000 →  Saldo: $0        Proveedor: $0  ✅ Pagada
```
