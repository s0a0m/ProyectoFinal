using Xunit;
using System.Linq;
using System.Collections.Generic;
using Dom = src.Models.Domain;
using EF = src.Models.CodeFirst;
using src.Models.Mappers;

namespace src.MapperTests;

public class ProveedorMapperTest
{
    private EF.Proveedor CrearProveedorEF(short id, string razonSocial, short idDomicilio, EF.Domicilio domicilio, short idCondicion, EF.CondicionDePago condicion)
    {
        return new EF.Proveedor
        {
            IdProveedor = id,
            RazonSocial = razonSocial,
            Cuit = $"20-{id}-123456-9",
            Telefono = "444555",
            Correo = $"contacto@{id}.com",
            PersonaResponsable = $"Resp {id}",
            Saldo = id * 100m,
            Activo = true,

            IdDomicilio = idDomicilio,
            IdDomicilioNavigation = domicilio,
            IdCondicionPagoHabitual = idCondicion,
            IdCondicionPagoHabitualNavigation = condicion,

            ProductosProveedores = new List<EF.ProductoProveedor>()
        };
    }
    private Dom.Proveedor CrearProveedorDom(int id, string razonSocial, Dom.Direccion direccion, Dom.CondicionDePago condicion)
    {
        return new Dom.Proveedor
        {
            IdProveedor = id,
            RazonSocial = razonSocial,
            Cuit = $"20-{id}-123456-9",
            Telefono = "444555",
            Correo = $"contacto@{id}.com",
            PersonaResponsable = $"Resp {id}",
            Saldo = id * 100m,
            Activo = true,

            Direccion = direccion,
            Condicion = condicion,
        };
    }

    private EF.Provincia CrearProvinciaEF(short id, string nombre)
    {
        return new EF.Provincia() { IdProvincia = id, Nombre = nombre };
    }
    private EF.CondicionDePago CrearContadoEF(short id)
    {
        return new EF.Contado { IdCondicionPago = id, DiasPago = 0 };
    }
    private Dom.Direccion CrearDireccionDom(short id, string calle, Dom.Provincia prov)
    {
        return new Dom.Direccion()
        {
            IdDomicilio = id,
            Calle = calle,
            Prov = prov
        };
    }
    private Dom.CondicionDePago CrearContadoDom(short id)
    {
        return new Dom.Contado { IdCondicionPago = id, DiasPago = 0 };
    }
    private Dom.Provincia CrearProvinciaDom(short id, string nombre)
    {
        return new Dom.Provincia()
        {
            IdProvincia = id,
            Nombre = nombre
        };
    }
    private EF.Domicilio CrearDireccionEF(short idDomicilio, string calle, short idProvincia, EF.Provincia prov)
    {
        return new EF.Domicilio()
        {
            IdDomicilio = idDomicilio,
            IdProvincia = idProvincia,
            IdProvinciaNavigation = prov,
            Calle = calle,
            Numero = (short)(100 + idDomicilio),
            Piso = (short)(idDomicilio % 2 == 0 ? 5 : 2),
            Comentario = $"Comentario EF {idDomicilio}"
        };
    }
    [Fact]
    public void ProveedorMapper_ListaDataADominio_MapeaRelacionesYCore()
    {
        // ARRANGE
        var efContado = CrearContadoEF(10);
        var efDomicilio1 = CrearDireccionEF(20, "Calle A", 1, CrearProvinciaEF(1, "Salta"));
        var efDomicilio2 = CrearDireccionEF(21, "Calle B", 2, CrearProvinciaEF(2, "Jujuy"));

        var efProveedores = new List<EF.Proveedor>
    {
        CrearProveedorEF(50, "Alfa S.R.L.", 20, efDomicilio1, 10, efContado),
        CrearProveedorEF(51, "Beta S.A.", 21, efDomicilio2, 10, efContado)
    };

        IEnumerable<Dom.Proveedor> domProveedores = DominioMapper.Map(efProveedores);

        Assert.Equal(2, domProveedores.Count());

        var provAlfa = domProveedores.First();

        Assert.Equal("Alfa S.R.L.", provAlfa.RazonSocial);
        Assert.Equal(5000m, provAlfa.Saldo);

        Assert.NotNull(provAlfa.Direccion);
        Assert.Equal("Calle A", provAlfa.Direccion.Calle);
        Assert.Equal(20, provAlfa.Direccion.IdDomicilio);

        Assert.NotNull(provAlfa.Condicion);
        Assert.Equal(10, provAlfa.Condicion.IdCondicionPago);
    }
    [Fact]
    public void ProveedorMapper_ListaDominioAData_MapeaFKsYCamposCore()
    {
        var domContado = CrearContadoDom(10);
        var domDireccion = CrearDireccionDom(20, "Calle Z", CrearProvinciaDom(1, "S. Fe"));

        var domProveedores = new List<Dom.Proveedor>
    {
        CrearProveedorDom(60, "Gamma E.S.", domDireccion, domContado),
    };

        IEnumerable<EF.Proveedor> efProveedores = DominioMapper.Map(domProveedores);

        Assert.Single(efProveedores);

        var provEF = efProveedores.First();

        Assert.Equal(60, provEF.IdProveedor);
        Assert.Equal("Gamma E.S.", provEF.RazonSocial);

        Assert.Equal(20, provEF.IdDomicilio);
        Assert.Equal(10, provEF.IdCondicionPagoHabitual);

        Assert.Empty(provEF.ProductosProveedores);
    }
}