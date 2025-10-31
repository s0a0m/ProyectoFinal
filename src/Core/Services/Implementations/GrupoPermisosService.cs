using src.Core.Services.Interfaces;
using src.Repositories.Interfaces;
using src.Presentation.ViewModels.GrupoPermisoVM;
using Dom = src.Models.Domain;

namespace src.Core.Services.Implementations;
public class GrupoPermisosService : IGrupoPermisosService
{
    private readonly IGrupoPermisosRepository _grupoRepo;
    private readonly IPermisoRepository _permisoRepo; // Para cargar la lista de permisos

    public GrupoPermisosService(IGrupoPermisosRepository grupoRepo, IPermisoRepository permisoRepo)
    {
        _grupoRepo = grupoRepo;
        _permisoRepo = permisoRepo;
    }

    public Task<IEnumerable<Dom.GrupoPermisos>> GetAllAsync()
    {
        return _grupoRepo.GetAllAsync();
    }

    public Task<Dom.GrupoPermisos?> GetByIdAsync(short id)
    {
        return _grupoRepo.GetByIdAsync(id);
    }
    
    public async Task<Dom.GrupoPermisos> CreateGrupoAsync(CrearGrupoViewModel vm)
    {
        // Mapear VM a Dominio
        var permisos = vm.PermisosSeleccionados
                         .Select(id => new Dom.Permiso { IdPermiso = id })
                         .ToList();

        var nuevoGrupo = new Dom.GrupoPermisos(0, vm.Nombre, vm.Descripcion, permisos);

        // Llamar al repositorio
        return await _grupoRepo.AddAsync(nuevoGrupo);
    }

    public async Task UpdateGrupoAsync(ActualizarGrupoViewModel vm)
    {
        // 1. Obtener entidad existente
        var grupoExistente = await _grupoRepo.GetByIdAsync(vm.IdGrupoPermiso);
        if (grupoExistente == null)
        {
            throw new KeyNotFoundException("Grupo de permisos no encontrado");
        }

        // 2. Mapear VM a Dominio
        var permisos = vm.PermisosSeleccionados
                         .Select(id => new Dom.Permiso { IdPermiso = id })
                         .ToList();
        
        var grupoActualizado = new Dom.GrupoPermisos(
            vm.IdGrupoPermiso, 
            vm.Nombre, 
            vm.Descripcion, 
            permisos
        );

        // 3. Actualizar escalares
        await _grupoRepo.UpdateAsync(grupoActualizado);
        
        // 4. Sincronizar permisos
        await _grupoRepo.ReemplazarPermisosAsync(vm.IdGrupoPermiso, vm.PermisosSeleccionados);
    }
    
    public async Task DeleteGrupoAsync(short id)
    {
        // (Aquí podrías verificar si el grupo está en uso por algún usuario)
        await _grupoRepo.DeleteAsync(id);
    }

    // --- Métodos de preparación de ViewModel (necesitarás crearlos) ---
    // (La lógica es idéntica a la de UserService, pero usando _permisoRepo)
    
    public async Task<CrearGrupoViewModel> PrepararCrearViewModelAsync()
    {
        var vm = new CrearGrupoViewModel();
        var todosLosPermisos = await _permisoRepo.GetAllPermisosAsync();
        vm.TodosLosPermisos = todosLosPermisos.Select(p => new PermisoAsignadoViewModel { ... }).ToList();
        return vm;
    }

    // ... etc. ...
}

