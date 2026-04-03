using Microsoft.AspNetCore.Mvc.Rendering;
using src.Presentation.ViewModels.FilaVM;
using src.Repositories.Interfaces;
using src.Core.Services.Interfaces;
using Dom = src.Models.Domain;

namespace src.Services.Implementations;

public class FilaService : IFilaService
{
    private readonly IFilaRepository    _filaRepo;
    private readonly IEstanteRepository _estanteRepo;

    public FilaService(IFilaRepository filaRepo, IEstanteRepository estanteRepo)
    {
        _filaRepo    = filaRepo;
        _estanteRepo = estanteRepo;
    }

    // ── Queries ──────────────────────────────────────────────────────────────

    public async Task<IEnumerable<FilaListItemViewModel>> GetByEstanteAsync(int idEstante)
    {
        try
        {
            var filas = await _filaRepo.GetByEstanteAsync(idEstante);
            return filas.Select(f => new FilaListItemViewModel
            {
                IdFila         = f.IdFila,
                NFila          = f.NFila,
                Activo         = f.Activo,
                TieneEspacio   = f.TieneEspacio,
                Observaciones  = f.Observaciones,
                IdEstante      = f.Estante.IdEstante,
                NumeroEstante  = f.Estante.NumeroEstante,
                IdDeposito     = f.Estante.Deposito.IdDeposito,
                NombreDeposito = f.Estante.Deposito.Nombre
            });
        }
        catch (Exception ex)
        {
            throw new ApplicationException(
                "No se pudo obtener la lista de filas.", ex);
        }
    }

    public async Task<FilaFormViewModel?> GetFormularioAsync(int id)
    {
        var f = await _filaRepo.GetByIdAsync(id);
        if (f is null) return null;

        var vm = new FilaFormViewModel
        {
            IdFila         = f.IdFila,
            NFila          = f.NFila,
            Activo         = f.Activo,
            TieneEspacio   = f.TieneEspacio,
            Observaciones  = f.Observaciones,
            IdEstante      = f.Estante.IdEstante,
            NumeroEstante  = f.Estante.NumeroEstante,
            IdDeposito     = f.Estante.Deposito.IdDeposito,
            NombreDeposito = f.Estante.Deposito.Nombre
        };
        await CargarOpcionesAsync(vm);
        return vm;
    }

    public async Task<FilaFormViewModel> GetFormularioVacioAsync(int? idEstante = null)
    {
        var vm = new FilaFormViewModel { Activo = true, TieneEspacio = true };

        if (idEstante.HasValue)
        {
            vm.IdEstante = idEstante.Value;
            var estante = await _estanteRepo.GetByIdAsync(idEstante.Value);
            if (estante is not null)
            {
                vm.NumeroEstante  = estante.NumeroEstante;
                vm.IdDeposito     = estante.Deposito.IdDeposito;
                vm.NombreDeposito = estante.Deposito.Nombre;
            }
        }

        await CargarOpcionesAsync(vm);
        return vm;
    }

    public async Task CargarOpcionesAsync(FilaFormViewModel vm)
    {
        try
        {
            // Carga todos los estantes activos; si hay depósito definido filtra por él
            IEnumerable<Dom.Estante> estantes;

            if (vm.IdDeposito > 0)
                estantes = await _estanteRepo.GetByDepositoAsync(vm.IdDeposito);
            else
                estantes = await _estanteRepo.GetAllAsync();

            vm.Estantes = estantes
                .Where(e => e.Activo)
                .Select(e => new SelectListItem
                {
                    Value    = e.IdEstante.ToString(),
                    Text     = $"{e.Deposito.Nombre}  —  Estante {e.NumeroEstante}",
                    Selected = e.IdEstante == vm.IdEstante
                });
        }
        catch
        {
            vm.Estantes = Enumerable.Empty<SelectListItem>();
        }
    }

    // ── Commands ─────────────────────────────────────────────────────────────

    public async Task<(bool success, string message)> CreateAsync(FilaFormViewModel vm)
    {
        try
        {
            var fila = new Dom.Fila
            {
                NFila         = vm.NFila.Trim(),
                Activo        = true,
                TieneEspacio  = vm.TieneEspacio,
                Observaciones = vm.Observaciones?.Trim(),
                Estante       = new Dom.Estante { IdEstante = vm.IdEstante }
            };

            await _filaRepo.AddAsync(fila);

            try { await _estanteRepo.SincronizarEspacioAsync(vm.IdEstante); } catch { }

            return (true, $"La fila \"{fila.NFila}\" fue creada correctamente.");
        }
        catch (Exception ex)
        {
            var inner = ex.InnerException?.Message ?? "";
            return (false, $"Error al crear la fila: {ex.Message}" +
                        (string.IsNullOrEmpty(inner) ? "" : $" ({inner})"));
        }
    }

    public async Task<(bool success, string message)> UpdateAsync(FilaFormViewModel vm)
    {
        try
        {
            var fila = new Dom.Fila
            {
                IdFila        = vm.IdFila,
                NFila         = vm.NFila.Trim(),
                Activo        = vm.Activo,
                TieneEspacio  = vm.TieneEspacio,
                Observaciones = vm.Observaciones?.Trim(),
                Estante       = new Dom.Estante { IdEstante = vm.IdEstante }
            };

            await _filaRepo.UpdateAsync(fila);
             await SincronizarEspacioEstanteAsync(vm.IdEstante);
            return (true, $"La fila \"{fila.NFila}\" fue actualizada correctamente.");
        }
        catch (KeyNotFoundException ex)
        {
            return (false, ex.Message);
        }
        catch (Exception)
        {
            return (false, "Error inesperado al actualizar la fila.");
        }
    }

    public async Task<(bool success, string message)> DeleteAsync(int id)
    {
        try
        {
            var fila = await _filaRepo.GetByIdAsync(id);
            if (fila is null)
                return (false, $"No se encontró la fila con ID {id}.");

            if (!fila.Activo)
                return (false, "La fila ya se encuentra inactiva.");

            bool tieneStock = fila.UbicacionProducto.Any(up => up.Cantidad > 0);
            if (tieneStock)
                return (false,
                    "No se puede desactivar la fila porque contiene productos con stock. " +
                    "Mueva o retire el stock antes de desactivarla.");

            await _filaRepo.DeleteAsync(id);
             await SincronizarEspacioEstanteAsync(fila.Estante.IdEstante);
            return (true, "La fila fue desactivada correctamente.");
        }
        catch (Exception)
        {
            return (false, "Error inesperado al desactivar la fila.");
        }
    }

    public async Task<(bool success, string message)> ReactivateAsync(int id)
    {
        try
        {
            // Obtener antes de reactivar para tener el IdEstante
            var fila = await _filaRepo.GetByIdAsync(id);
            if (fila is null)
                return (false, $"No se encontró la fila con ID {id}.");

            int idEstante = fila.Estante.IdEstante;

            await _filaRepo.ReactivateAsync(id);

            await SincronizarEspacioEstanteAsync(idEstante);

            return (true, "La fila fue reactivada correctamente.");
        }
        catch (Exception ex) when (ex.Message.Contains("No se encontró"))
        {
            return (false, ex.Message);
        }
        catch (Exception) { return (false, "Error inesperado al reactivar la fila."); }
    }

    private async Task SincronizarEspacioEstanteAsync(int idEstante)
    {
        try
        {
            await _estanteRepo.SincronizarEspacioAsync(idEstante);
        }
        catch
        {
            // No crítico: la fila ya fue modificada correctamente.
        }
    }
}