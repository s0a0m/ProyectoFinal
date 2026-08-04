using src.Contracts;
using src.Core.Contracts;
using src.Core.Services.Interfaces;
using src.Models.Domain;
using src.Presentation.ViewModels.CuentaCorrienteVM;
using src.Repositories.Interfaces;
using src.ViewModels;
using Dom = src.Models.Domain;

namespace src.Core.Services.Implementations;

public class ProveedorService : IProveedorService
{
    private readonly IProveedorRepository _proveedorRepository;
    private readonly ICommonDataService _commonDataService;
    private readonly IExcelDataReader _excelReader;
    IProductoProveedorService _prodService;
    IFacturaRepository _facturaRepo;
    IOrdenPagoRepository _ordenPagoRepository;
    IComprobanteRepository _comprobanteRepository;

    public ProveedorService(
        IExcelDataReader reader,
        IProveedorRepository proveedorRepository,
        ICommonDataService _commonDataService,
        IProductoProveedorService proProvSv,
        IFacturaRepository facturaRepo,
        IOrdenPagoRepository ordenPagoRepository,
        IComprobanteRepository comprobanteRepository
    )
    {
        _proveedorRepository = proveedorRepository;
        this._commonDataService = _commonDataService;
        this._excelReader = reader;
        _prodService = proProvSv;
        _facturaRepo = facturaRepo;
        _ordenPagoRepository = ordenPagoRepository;
        _comprobanteRepository = comprobanteRepository;
    }

    public async Task<Dom.Proveedor> CreateProveedorAsync(CrearProveedorViewModel proveedorVM)
    {
        Dom.Proveedor proveedor = CrearProveedorViewModel.cargarProveedor(proveedorVM);

        // Agregar despues: Lógica de Negocio (Ej. validar CUIT si es necesario)

        Dom.Provincia? provincia = await _commonDataService.GetProvinciaByIdAsync(
            proveedor.Direccion.Prov.IdProvincia
        );

        if (provincia == null)
        {
            throw new ArgumentException(
                "La provincia seleccionada no es válida.",
                nameof(proveedor.Direccion.Prov.IdProvincia)
            );
        }

        proveedor.Direccion.Prov = provincia;
        proveedor.Activo = true;

        await _proveedorRepository.AddAsync(proveedor);
        return proveedor;
    }

    public async Task DeleteAsync(int id)
    {
        // 1. Desactivamos el Proveedor en su propia tabla
        bool eliminado = await _proveedorRepository.DeleteAsync(id);

        if (!eliminado)
        {
            throw new KeyNotFoundException($"Proveedor con ID {id} no encontrado.");
        }

        // 2. PROPAGACIÓN: Avisamos a la relación que se apague (Cascada)
        // false = estamos desactivando
        await _prodService.GestionarCascadaProveedorAsync(id, false);
    }

    // NUEVO: Método Reactivate con propagación
    public async Task ReactivateAsync(int id)
    {
        // 1. Reactivamos el Proveedor
        bool reactivado = await _proveedorRepository.ReactivateAsync(id);

        if (!reactivado)
        {
            throw new KeyNotFoundException($"Proveedor con ID {id} no encontrado.");
        }

        // 2. PROPAGACIÓN: Avisamos a la relación que intente revivir
        // true = estamos activando (el servicio validará si el producto está activo)
        await _prodService.GestionarCascadaProveedorAsync(id, true);
    }

    public async Task<IEnumerable<Proveedor>> GetActiveProveedoresAsync()
    {
        var proveedores = await _proveedorRepository.GetAllProveedorAsync();
        return proveedores;
    }

    public async Task<Proveedor> GetProveedorByIdAsync(int IdProveedor)
    {
        Dom.Proveedor? proveedor = await _proveedorRepository.GetProveedorById(IdProveedor);

        if (proveedor == null)
        {
            throw new KeyNotFoundException($"Proveedor con ID {IdProveedor} no encontrado.");
        }
        return proveedor;
    }

    // src/Core/Services/Implementations/ProveedorService.cs

    public async Task UpdateProveedorAsync(ActualizarProveedorViewModel proveedorVM)
    {
        Dom.Proveedor proveedorExistente;
        try
        {
            proveedorExistente = await GetProveedorByIdAsync(proveedorVM.IdProveedor);
        }
        catch (KeyNotFoundException)
        {
            throw;
        }

        // Mapeo de propiedades simples
        proveedorExistente.Cuit = proveedorVM.Cuit.Trim();
        proveedorExistente.RazonSocial = proveedorVM.RazonSocial.Trim();
        proveedorExistente.Telefono = proveedorVM.Telefono.Trim();
        proveedorExistente.Correo = proveedorVM.Correo.Trim();
        proveedorExistente.PersonaResponsable = proveedorVM.PersonaResponsable.Trim();
        proveedorExistente.Saldo = proveedorVM.Saldo;

        // Mapeo de Dirección
        proveedorExistente.Direccion.Calle = proveedorVM.Direccion.calle.Trim();
        proveedorExistente.Direccion.Numero = proveedorVM.Direccion.numero;
        proveedorExistente.Direccion.Piso = proveedorVM.Direccion.piso;
        proveedorExistente.Direccion.Comentario = proveedorVM.Direccion.comentario?.Trim();

        Dom.Provincia? provinciaSeleccionada = await _commonDataService.GetProvinciaByIdAsync(
            proveedorVM.Direccion.provincia.Id_provincia
        );

        if (provinciaSeleccionada == null)
        {
            throw new ArgumentException(
                "La provincia seleccionada no es válida.",
                "Direccion.provincia.Id_provincia"
            );
        }

        proveedorExistente.Direccion.Prov = provinciaSeleccionada;

        if (proveedorVM.CondicionPago.Tipo == "Cuota")
        {
            var cuota = (proveedorExistente.Condicion as Dom.Cuota) ?? new Dom.Cuota();

            cuota.DiasPago = proveedorVM.CondicionPago.DiasPago;
            cuota.Cuotas = proveedorVM.CondicionPago.NumeroCuotas;
            cuota.InteresPorcentual = proveedorVM.CondicionPago.InteresPorcentual;

            proveedorExistente.Condicion = cuota;
        }
        else if (proveedorVM.CondicionPago.Tipo == "Contado")
        {
            var contado = (proveedorExistente.Condicion as Dom.Contado) ?? new Dom.Contado();

            contado.DiasPago = proveedorVM.CondicionPago.DiasPago;

            proveedorExistente.Condicion = contado;
        }
        else
        {
            throw new ArgumentException(
                "El tipo de condición de pago no es válido.",
                "CondicionPago.Tipo"
            );
        }

        await _proveedorRepository.UpdateAsync(proveedorExistente);
    }

    public async Task<CuentaCorrienteData?> ObtenerCuentaCorrienteAsync(short idProveedor)
    {
        var proveedor = await _proveedorRepository.GetProveedorById(idProveedor);
        if (proveedor == null)
            return null;

        var facturas = await _facturaRepo.GetByProveedorAsync(idProveedor);
        var ordenes = await _ordenPagoRepository.GetByProveedorAsync(idProveedor);
        var comprobantes = await _comprobanteRepository.GetByProveedorAsync(idProveedor);
        var motivosNC = await _comprobanteRepository.GetMotivosAsync("NC");
        var motivosND = await _comprobanteRepository.GetMotivosAsync("ND");

        return new CuentaCorrienteData
        {
            Proveedor = proveedor,
            Facturas = facturas.ToList(),
            OrdenesPago = ordenes.Where(o => o.Enviada).ToList(),
            Comprobantes = comprobantes.ToList(),
            MotivosNC = motivosNC.ToList(),
            MotivosND = motivosND.ToList(),
        };
    }
    // public async Task<CuentaCorrienteData?> ObtenerCuentaCorrienteAsync(short idProveedor)
    // {
    //     var proveedor = await _proveedorRepository.GetProveedorById(idProveedor);
    //     if (proveedor == null)
    //         return null;
    //
    //     var facturas = await _facturaRepo.GetByProveedorAsync(idProveedor);
    //     var ordenes = await _ordenPagoRepository.GetByProveedorAsync(idProveedor);
    //     var comprobantes = await _comprobanteRepository.GetByProveedorAsync(idProveedor);
    //
    //     return new CuentaCorrienteData
    //     {
    //         Proveedor = proveedor,
    //         Facturas = facturas.ToList(),
    //         OrdenesPago = ordenes.Where(o => o.Enviada).ToList(),
    //         Comprobantes = comprobantes.ToList(),
    //     };
    // }
}
