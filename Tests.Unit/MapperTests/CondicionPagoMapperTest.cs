using Xunit;
using System.Linq;
using Dom = src.Models.Domain;
using EF = src.Models.CodeFirst;
using src.Models.Mappers;

namespace src.MapperTests;

public class CondicionPagoMapperTest
{
    [Fact]
    public void CondicionPagoMapper_ListaDominioAData_MapeaHerenciaYDatos()
    {
        var domCondiciones = new List<Dom.CondicionDePago>
        {
            CrearCuotaDom(id: 20, cuotas: 6, interes: 12.00m),
            CrearContadoDom(id: 21),
            CrearCuotaDom(id: 23, cuotas: 122, interes: 32),
            CrearContadoDom(id: 24)
        };

        IEnumerable<EF.CondicionDePago> efCondiciones = DominioMapper.Map(domCondiciones);

        Assert.Equal(4, efCondiciones.Count());

        var efCondicionCuota = efCondiciones.OfType<EF.Cuota>().First();
        var efCondicionContado = efCondiciones.OfType<EF.Contado>().Last();

        Assert.IsType<EF.Cuota>(efCondicionCuota);
        Assert.Equal(20, efCondicionCuota.IdCondicionPago);
        Assert.Equal(6, efCondicionCuota.Cuotas);
        Assert.Equal(12.00m, efCondicionCuota.InteresPorcentual);

        Assert.IsType<EF.Contado>(efCondicionContado);
        Assert.Equal(24, efCondicionContado.IdCondicionPago);
        Assert.Equal(6, efCondicionCuota.Cuotas);
        Assert.Equal(12.00m, efCondicionCuota.InteresPorcentual);
    }
    [Fact]
    public void CondicionPagoMapper_ListaDataADominio_MapeaHerenciaYDatos()
    {
        var efCondiciones = new List<EF.CondicionDePago>
        {
            CrearContadoEF(id: 10),
            CrearCuotaEF(id: 11, cuotas: 3, interes: 5.50m),
            CrearContadoEF(id: 12)
        };

        IEnumerable<Dom.CondicionDePago> domCondiciones = DominioMapper.Map(efCondiciones);

        Assert.Equal(3, domCondiciones.Count());

        var condicionCuota = domCondiciones.OfType<Dom.Cuota>().First();

        Assert.IsType<Dom.Cuota>(condicionCuota);
        Assert.Equal(11, condicionCuota.IdCondicionPago);
        Assert.Equal(3, condicionCuota.Cuotas);
        Assert.Equal(5.50m, condicionCuota.InteresPorcentual);
        Assert.Equal(30, condicionCuota.DiasPago);

        var condicionContado = domCondiciones.OfType<Dom.Contado>().First();
        Assert.IsType<Dom.Contado>(condicionContado);
        Assert.Equal(10, condicionContado.IdCondicionPago);
        Assert.Equal(0, condicionContado.DiasPago);
    }
    private EF.Contado CrearContadoEF(short id)
    {
        return new EF.Contado
        {
            IdCondicionPago = id,
            DiasPago = 0
        };
    }

    private EF.Cuota CrearCuotaEF(short id, short cuotas, decimal interes)
    {
        return new EF.Cuota
        {
            IdCondicionPago = id,
            DiasPago = 30,
            Cuotas = cuotas,
            InteresPorcentual = interes
        };
    }
    private Dom.Contado CrearContadoDom(short id)
    {
        return new Dom.Contado
        {
            IdCondicionPago = id,
            DiasPago = 0
        };
    }

    private Dom.Cuota CrearCuotaDom(short id, short cuotas, decimal interes)
    {
        return new Dom.Cuota
        {
            IdCondicionPago = id,
            DiasPago = 30,
            Cuotas = cuotas,
            InteresPorcentual = interes
        };
    }
}