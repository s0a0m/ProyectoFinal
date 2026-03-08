using Microsoft.AspNetCore.Mvc.Rendering;
using src.Presentation.ViewModels.DepositoVM;
using src.Core.Services.Interfaces;
using Dom = src.Models.Domain;
using src.Repositories.Interfaces;   
namespace src.Core.Services.Implementations;

public class DepositoService : IDepositoService
{
    private readonly IDepositoRepository  _depositoRepo;
    private readonly IProvinciaRepository _provinciaRepo;
    private readonly IEstanteRepository   _estanteRepo;   // ← NUEVO
    private readonly IFilaRepository      _filaRepo;      // ← NUEVO
    private readonly IUbicacionProductoRepository _ubicacionRepo;

    public DepositoService(
        IDepositoRepository  depositoRepo,
        IProvinciaRepository provinciaRepo,
        IEstanteRepository   estanteRepo,
        IFilaRepository      filaRepo,
        IUbicacionProductoRepository ubicacionRepo)
    {
        _depositoRepo  = depositoRepo;
        _provinciaRepo = provinciaRepo;
        _estanteRepo   = estanteRepo;
        _filaRepo      = filaRepo;
        _ubicacionRepo = ubicacionRepo;
    }

    // ── Queries ───────────────────────────────────────────────────────────────

    public async Task<IEnumerable<DepositoListItemViewModel>> GetAllAsync()
    {
        try
        {
            var depositos = await _depositoRepo.GetAllAsync();
            var conteos   = await _depositoRepo.GetConteosAsync();   // ← NUEVO

            return depositos.Select(d =>
            {
                conteos.TryGetValue(d.IdDeposito, out var c);
                return new DepositoListItemViewModel
                {
                    IdDeposito       = d.IdDeposito,
                    Nombre           = d.Nombre,
                    Activo           = d.Activo,
                    DireccionResumen = FormatearDireccion(d.Direccion),
                    TotalEstantes    = c.Estantes,   // viene de la BD, no del mapper
                    TotalFilas       = c.Filas,
                    Calle       = d.Direccion?.Calle       ?? string.Empty,
                    Numero      = d.Direccion?.Numero      ?? 0,
                    Piso        = d.Direccion?.Piso,
                    Comentario  = d.Direccion?.Comentario  ?? string.Empty,
                    IdProvincia = d.Direccion?.Prov?.IdProvincia ?? 0
                };
            });
        }
        catch (Exception ex)
        {
            throw new ApplicationException(
                "No se pudo obtener la lista de depósitos.", ex);
        }
    }


    public async Task<DetalleDepositoViewModel?> GetDetalleAsync(int id)
    {
        try
        {
            var d = await _depositoRepo.GetByIdAsync(id);
            if (d is null) return null;

            var estantesDomain = await _estanteRepo.GetByDepositoAsync(id);
            var estantesVm     = new List<EstanteDetalleViewModel>();
            var productosDict  = new Dictionary<int, ProductoResumenDepositoViewModel>();

            foreach (var estante in estantesDomain)
            {
                var filasDomain = await _filaRepo.GetByEstanteAsync(estante.IdEstante);
                var filasVm     = new List<FilaDetalleViewModel>();

                foreach (var fila in filasDomain)
                {
                    // ← Consultar ubicaciones directamente, no confiar en el mapper
                    var ubicaciones = await _ubicacionRepo.GetByFilaAsync(fila.IdFila);
                    decimal stockFila = ubicaciones.Sum(up => up.Cantidad);

                    filasVm.Add(new FilaDetalleViewModel
                    {
                        IdFila         = fila.IdFila,
                        NFila          = fila.NFila,
                        Activo         = fila.Activo,
                        TieneEspacio   = fila.TieneEspacio,
                        Observaciones  = fila.Observaciones,
                        TotalStockFila = stockFila
                    });

                    // Consolidar productos del depósito
                    foreach (var ub in ubicaciones)
                    {
                        if (ub.Cantidad <= 0) continue;

                        if (!productosDict.TryGetValue(ub.Producto.IdProducto, out var pVm))
                        {
                            pVm = new ProductoResumenDepositoViewModel
                            {
                                IdProducto     = ub.Producto.IdProducto,
                                NombreProducto = ub.Producto.Nombre
                            };
                            productosDict[ub.Producto.IdProducto] = pVm;
                        }
                        pVm.StockEnDeposito += ub.Cantidad;
                        pVm.CantidadFilas++;
                    }
                }

                estantesVm.Add(new EstanteDetalleViewModel
                {
                    IdEstante     = estante.IdEstante,
                    NumeroEstante = estante.NumeroEstante,
                    Activo        = estante.Activo,
                    TieneEspacio  = estante.TieneEspacio,
                    Observaciones = estante.Observaciones,
                    Filas         = filasVm.OrderBy(f => f.NFila)
                });
            }
            var formulario = await GetFormularioAsync(id);
            if(formulario is null)
                throw new ApplicationException("No se pudo cargar el formulario de edición del depósito.");
            return new DetalleDepositoViewModel
            {
                IdDeposito       = d.IdDeposito,
                Nombre           = d.Nombre,
                Activo           = d.Activo,
                DireccionResumen = FormatearDireccion(d.Direccion),
                Estantes         = estantesVm.OrderBy(e => e.NumeroEstante),
                Productos        = productosDict.Values.OrderBy(p => p.NombreProducto),
                FormularioEdicion = formulario

            };
        }
        catch (Exception ex)
        {
            throw new ApplicationException("No se pudo cargar el detalle del depósito.", ex);
        }
    }



    public async Task<DepositoFormViewModel> GetFormularioVacioAsync()
    {
        var vm = new DepositoFormViewModel { Activo = true };
        await CargarOpcionesAsync(vm);
        return vm;
    }

    public async Task<DepositoFormViewModel?> GetFormularioAsync(int id)
    {
        var d = await _depositoRepo.GetByIdAsync(id);
        if (d is null) return null;

        var vm = new DepositoFormViewModel
        {
            IdDeposito  = d.IdDeposito,
            Nombre      = d.Nombre,
            Activo      = d.Activo,
            // Dirección
            Calle       = d.Direccion?.Calle       ?? string.Empty,
            Numero      = d.Direccion?.Numero      ?? 0,
            Piso        = d.Direccion?.Piso,
            Comentario  = d.Direccion?.Comentario,
            // Provincia: se lee desde el objeto Prov ya mapeado
            IdProvincia = d.Direccion?.Prov?.IdProvincia ?? 0,
        };

        await CargarOpcionesAsync(vm);
        return vm;
    }

    public async Task CargarOpcionesAsync(DepositoFormViewModel vm)
    {
        try
        {
            var provincias = await _provinciaRepo.GetAllAsync();
            vm.Provincias = provincias
                .OrderBy(p => p.Nombre)
                .Select(p => new SelectListItem
                {
                    Value    = p.IdProvincia.ToString(),
                    Text     = p.Nombre,
                    Selected = p.IdProvincia == vm.IdProvincia
                });
        }
        catch
        {
            // Si falla la carga de provincias el formulario igual se renderiza;
            // el Required del select se encargará de bloquear el submit.
            vm.Provincias = Enumerable.Empty<SelectListItem>();
        }
    }

    // ── Commands ──────────────────────────────────────────────────────────────

    public async Task<(bool success, string message)> CreateAsync(DepositoFormViewModel vm)
    {
        try
        {
            var deposito = new Dom.Deposito
            {
                Nombre = vm.Nombre.Trim(),
                Activo = true,
                Direccion = new Dom.Direccion
                {
                    Calle      = vm.Calle.Trim(),
                    Numero     = vm.Numero,
                    Piso       = vm.Piso,
                    Comentario = vm.Comentario?.Trim(),
                    Prov       = new Dom.Provincia { IdProvincia = vm.IdProvincia }
                }
            };

            await _depositoRepo.AddAsync(deposito);
            return (true, $"El depósito \"{deposito.Nombre}\" fue creado correctamente.");
        }
        catch (InvalidOperationException ex)
        {
            return (false, ex.Message);
        }
        catch (Exception)
        {
            return (false, "Ocurrió un error inesperado al crear el depósito. Intente nuevamente.");
        }
    }

    public async Task<(bool success, string message)> UpdateAsync(DepositoFormViewModel vm)
    {
        try
        {
            var deposito = await _depositoRepo.GetByIdAsync(vm.IdDeposito);
            if (deposito is null)
                return (false, $"No se encontró el depósito con ID {vm.IdDeposito}.");

            deposito.Nombre = vm.Nombre.Trim();
            deposito.Activo = vm.Activo;

            // Actualizamos la dirección sobre el objeto ya cargado
            deposito.Direccion ??= new Dom.Direccion();
            deposito.Direccion.Calle      = vm.Calle.Trim();
            deposito.Direccion.Numero     = vm.Numero;
            deposito.Direccion.Piso       = vm.Piso;
            deposito.Direccion.Comentario = vm.Comentario?.Trim();
            deposito.Direccion.Prov       = new Dom.Provincia { IdProvincia = vm.IdProvincia };

            await _depositoRepo.UpdateAsync(deposito);
            return (true, $"El depósito \"{deposito.Nombre}\" fue actualizado correctamente.");
        }
        catch (KeyNotFoundException ex)
        {
            return (false, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return (false, ex.Message);
        }
        catch (Exception)
        {
            return (false, "Ocurrió un error inesperado al actualizar el depósito.");
        }
    }

    public async Task<(bool success, string message)> DeleteAsync(int id)
    {
        try
        {
            var deposito = await _depositoRepo.GetByIdAsync(id);
            if (deposito is null)
                return (false, $"No se encontró el depósito con ID {id}.");

            if (!deposito.Activo)
                return (false, "El depósito ya se encuentra inactivo.");

            bool tieneStock = deposito.Estantes
                .Where(e => e.Activo)
                .SelectMany(e => e.Filas.Where(f => f.Activo))
                .SelectMany(f => f.UbicacionProducto)
                .Any(up => up.Cantidad > 0);

            if (tieneStock)
                return (false,
                    "No se puede dar de baja el depósito porque tiene productos con stock. " +
                    "Mueva el stock a otro depósito antes de desactivarlo.");

            await _depositoRepo.DeleteAsync(id);
            return (true, $"El depósito \"{deposito.Nombre}\" fue dado de baja correctamente.");
        }
        catch (Exception)
        {
            return (false, "Ocurrió un error inesperado al intentar dar de baja el depósito.");
        }
    }

    public async Task<(bool success, string message)> ReactivateAsync(int id)
    {
        try
        {
            await _depositoRepo.ReactivateAsync(id);
            return (true, "El depósito fue reactivado correctamente.");
        }
        catch (Exception ex) when (ex.Message.Contains("No se encontró"))
        {
            return (false, ex.Message);
        }
        catch (Exception)
        {
            return (false, "Error inesperado al reactivar el depósito.");
        }
    }

    public async Task<IEnumerable<SelectListItem>> GetProvinciasAsync()
    {
        try
        {
            var provincias = await _provinciaRepo.GetAllAsync();
            return provincias
                .OrderBy(p => p.Nombre)
                .Select(p => new SelectListItem
                {
                    Value = p.IdProvincia.ToString(),
                    Text  = p.Nombre
                });
        }
        catch
        {
            return Enumerable.Empty<SelectListItem>();
        }
    }

    // ── Helpers privados ──────────────────────────────────────────────────────

    private static DepositoListItemViewModel MapToListItem(Dom.Deposito d) => new()
    {
        IdDeposito       = d.IdDeposito,
        Nombre           = d.Nombre,
        Activo           = d.Activo,
        DireccionResumen = FormatearDireccion(d.Direccion),
        TotalEstantes    = d.Estantes?.Count() ?? 0,
        TotalFilas       = d.Estantes?.Sum(e => e.Filas?.Count() ?? 0) ?? 0
    };

    /// Arma una cadena legible: "Calle 123, Piso 2  •  Buenos Aires"
    private static string FormatearDireccion(Dom.Direccion? dir)
    {
        if (dir is null) return "Sin dirección";

        // Parte calle + número
        var calleNumero = $"{dir.Calle} {dir.Numero}".Trim();

        // Piso opcional
        var piso = dir.Piso.HasValue ? $", Piso {dir.Piso}" : string.Empty;

        // Provincia
        var provincia = dir.Prov is not null ? $"  •  {dir.Prov.Nombre}" : string.Empty;

        return $"{calleNumero}{piso}{provincia}";
    }
}
