using Microsoft.AspNetCore.Mvc.Rendering;
using src.Presentation.ViewModels.EstanteVM;
using src.Repositories.Interfaces;
using src.Core.Services.Interfaces;
using Dom = src.Models.Domain;

namespace src.Services.Implementations;

public class EstanteService : IEstanteService
{
    private readonly IEstanteRepository  _estanteRepo;
    private readonly IDepositoRepository _depositoRepo;

    public EstanteService(
        IEstanteRepository  estanteRepo,
        IDepositoRepository depositoRepo)
    {
        _estanteRepo  = estanteRepo;
        _depositoRepo = depositoRepo;
    }

    // ── Queries ──────────────────────────────────────────────────────────────

    public async Task<IEnumerable<EstanteListItemViewModel>> GetByDepositoAsync(int idDeposito)
    {
        try
        {
            var estantes = await _estanteRepo.GetByDepositoAsync(idDeposito);
            return estantes.Select(e => new EstanteListItemViewModel
            {
                IdEstante      = e.IdEstante,
                NumeroEstante  = e.NumeroEstante,
                Activo         = e.Activo,
                TieneEspacio   = e.TieneEspacio,
                Observaciones  = e.Observaciones,
                IdDeposito     = e.Deposito.IdDeposito,
                NombreDeposito = e.Deposito.Nombre,
                TotalFilas     = e.Filas?.Count() ?? 0
            });
        }
        catch (Exception ex)
        {
            throw new ApplicationException(
                "No se pudo obtener la lista de estantes.", ex);
        }
    }

    public async Task<EstanteFormViewModel?> GetFormularioAsync(int id)
    {
        var e = await _estanteRepo.GetByIdAsync(id);
        if (e is null) return null;

        var vm = new EstanteFormViewModel
        {
            IdEstante      = e.IdEstante,
            NumeroEstante  = e.NumeroEstante,
            Activo         = e.Activo,
            TieneEspacio   = e.TieneEspacio,
            Observaciones  = e.Observaciones,
            IdDeposito     = e.Deposito.IdDeposito,
            NombreDeposito = e.Deposito.Nombre,
        };
        await CargarOpcionesAsync(vm);
        return vm;
    }

    public async Task<EstanteFormViewModel> GetFormularioVacioAsync(int? idDeposito = null)
    {
        var vm = new EstanteFormViewModel { Activo = true, TieneEspacio = true };

        if (idDeposito.HasValue)
        {
            vm.IdDeposito = idDeposito.Value;
            var dep = await _depositoRepo.GetByIdAsync(idDeposito.Value);
            vm.NombreDeposito = dep?.Nombre ?? string.Empty;
        }

        await CargarOpcionesAsync(vm);
        return vm;
    }

    public async Task CargarOpcionesAsync(EstanteFormViewModel vm)
    {
        try
        {
            var depositos = await _depositoRepo.GetActivosAsync();
            vm.Depositos = depositos.Select(d => new SelectListItem
            {
                Value    = d.IdDeposito.ToString(),
                Text     = d.Nombre,
                Selected = d.IdDeposito == vm.IdDeposito
            });
        }
        catch
        {
            vm.Depositos = Enumerable.Empty<SelectListItem>();
        }
    }

    // ── Commands ─────────────────────────────────────────────────────────────

    public async Task<(bool success, string message)> CreateAsync(EstanteFormViewModel vm)
    {
        try
        {
            var estante = new Dom.Estante
            {
                NumeroEstante = vm.NumeroEstante.Trim(),
                Activo        = true,
                TieneEspacio  = vm.TieneEspacio,
                Observaciones = vm.Observaciones?.Trim(),
                Deposito      = new Dom.Deposito { IdDeposito = vm.IdDeposito }
            };

            await _estanteRepo.AddAsync(estante);

            // SincronizarEspacio es silencioso — si no está implementado, no importa
            try { await _estanteRepo.SincronizarEspacioAsync(estante.IdEstante); } catch { }

            return (true, $"El estante \"{estante.NumeroEstante}\" fue creado correctamente.");
        }
        catch (Exception ex)
        {
            // Retorna el mensaje real — así lo ves en TempData["error"]
            var inner = ex.InnerException?.Message ?? "";
            return (false, $"Error al crear el estante: {ex.Message}" +
                        (string.IsNullOrEmpty(inner) ? "" : $" ({inner})"));
        }
    }


    public async Task<(bool success, string message)> UpdateAsync(EstanteFormViewModel vm)
    {
        try
        {
            var estante = new Dom.Estante
            {
                IdEstante     = vm.IdEstante,
                NumeroEstante = vm.NumeroEstante.Trim(),
                Activo        = vm.Activo,
                TieneEspacio  = vm.TieneEspacio,
                Observaciones = vm.Observaciones?.Trim(),
                Deposito      = new Dom.Deposito { IdDeposito = vm.IdDeposito }
            };

            await _estanteRepo.UpdateAsync(estante);
            return (true, $"El estante \"{estante.NumeroEstante}\" fue actualizado correctamente.");
        }
        catch (KeyNotFoundException ex)
        {
            return (false, ex.Message);
        }
        catch (Exception)
        {
            return (false, "Error inesperado al actualizar el estante.");
        }
    }

    public async Task<(bool success, string message)> DeleteAsync(int id)
    {
        try
        {
            var estante = await _estanteRepo.GetByIdAsync(id);
            if (estante is null)
                return (false, $"No se encontró el estante con ID {id}.");

            if (!estante.Activo)
                return (false, "El estante ya se encuentra inactivo.");

            bool tieneStock = estante.Filas
                .SelectMany(f => f.UbicacionProducto)
                .Any(up => up.Cantidad > 0);

            if (tieneStock)
                return (false,
                    "No se puede desactivar el estante porque contiene filas con stock. " +
                    "Mueva el stock antes de desactivarlo.");

            await _estanteRepo.DeleteAsync(id);
            return (true, "El estante fue desactivado correctamente.");
        }
        catch (Exception)
        {
            return (false, "Error inesperado al desactivar el estante.");
        }
    }

    public async Task<(bool success, string message)> ReactivateAsync(int id)
    {
        try
        {
            await _estanteRepo.ReactivateAsync(id);
            return (true, "El estante fue reactivado correctamente.");
        }
        catch (Exception ex) when (ex.Message.Contains("No se encontró"))
        {
            return (false, ex.Message);
        }
        catch (Exception)
        {
            return (false, "Error inesperado al reactivar el estante.");
        }
    }

    public async Task<(bool success, string message)> MarcarEspacioAsync(int id, bool tieneEspacio)
    {
        try
        {
            await _estanteRepo.MarcarEstanteLlenoAsync(id, tieneEspacio);
            string estado = tieneEspacio ? "disponible" : "lleno";
            return (true, $"El espacio del estante fue marcado como {estado}.");
        }
        catch (KeyNotFoundException ex)
        {
            return (false, ex.Message);
        }
        catch (Exception)
        {
            return (false, "Error al actualizar el espacio del estante.");
        }
    }

    public async Task SincronizarEspacioAsync(int idEstante)
    {
        // Delega directamente al repositorio; los errores suben sin wrap
        // para que el llamador decida si loguea o ignora silenciosamente.
        await _estanteRepo.SincronizarEspacioAsync(idEstante);
    }

}