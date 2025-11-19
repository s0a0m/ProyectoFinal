using src.Core.Services.Interfaces;
using src.Repositories.Interfaces;
using src.Presentation.ViewModels.GrupoPermisoVM;
using src.Presentation.ViewModels.UsuarioVM;
using Dom = src.Models.Domain;
using src.Repositories.Implementations;


namespace src.Core.Services.Implementations
{
    public class GrupoPermisosService : IGrupoPermisosService
    {
        private readonly IGrupoPermisosRepository _grupoRepo;
        private readonly IPermisoRepository _permisoRepo;

        public GrupoPermisosService(IGrupoPermisosRepository grupoRepo, IPermisoRepository permisoRepo)
        {
            _grupoRepo = grupoRepo;
            _permisoRepo = permisoRepo;
        }

        public Task<IEnumerable<Dom.GrupoPermisos>> GetAllAsync()
        {
            return _grupoRepo.GetAllAsync();
        }

       public async Task<Dom.GrupoPermisos> GetByIdAsync(short id)
        {
            var grupo = await _grupoRepo.GetByIdAsync(id);
            
            if (grupo == null)
            {
                // Lanzamos la excepción que el controlador espera
                throw new KeyNotFoundException($"Grupo de permisos con ID {id} no encontrado.");
            }

            return grupo;
            }
        
        public async Task<Dom.GrupoPermisos> CreateGrupoAsync(CrearGrupoViewModel vm)
        {
            var permisos = vm.PermisosSeleccionados
                                .Select(id => new Dom.Permiso { IdPermiso = id })
                                .ToList();

            var nuevoGrupo = new Dom.GrupoPermisos(0, vm.Nombre.Trim(), vm.Descripcion?.Trim() ?? string.Empty, permisos);

            return await _grupoRepo.AddAsync(nuevoGrupo);
        }

        public async Task UpdateGrupoAsync(ActualizarGrupoViewModel vm)
        {
            var grupoExistente = await _grupoRepo.GetByIdAsync(vm.IdGrupoPermiso);
            if (grupoExistente == null)
            {
                throw new KeyNotFoundException("Grupo de permisos no encontrado");
            }

            var permisos = vm.PermisosSeleccionados
                                .Select(id => new Dom.Permiso { IdPermiso = id })
                                .ToList();
            
            var grupoActualizado = new Dom.GrupoPermisos(
                vm.IdGrupoPermiso, 
                vm.Nombre.Trim(), 
                vm.Descripcion?.Trim() ?? string.Empty, 
                permisos
            );

            // 1. Actualizar escalares (Nombre, Descripcion)
            await _grupoRepo.UpdateAsync(grupoActualizado);
            
            // 2. Sincronizar permisos (N-a-N)
            await _grupoRepo.ReemplazarPermisosAsync(vm.IdGrupoPermiso, vm.PermisosSeleccionados);
        }
        
        public async Task DeleteGrupoAsync(short id)
        {
            // (Aquí podrías verificar si el grupo está en uso por algún usuario)
            await _grupoRepo.DeleteAsync(id);
        }

        // --- MÉTODOS DE PREPARACIÓN DE VIEWMODEL (COMPLETADOS) ---
        
        public async Task<CrearGrupoViewModel> PrepararCrearViewModelAsync()
        {
            var vm = new CrearGrupoViewModel();
            var todosLosPermisos = await _permisoRepo.GetAllPermisosAsync();
            
            // --- ESTA ES LA LÍNEA CORREGIDA ---
            vm.TodosLosPermisos = todosLosPermisos.Select(p => new PermisoAsignadoViewModel 
            { 
                IdPermiso = p.IdPermiso,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                Asignado = false // En 'Crear', ninguno está asignado
            }).ToList();
            
            return vm;
        }

        public async Task<ActualizarGrupoViewModel> PrepararActualizarViewModelAsync(short id)
        {
            var grupo = await _grupoRepo.GetByIdAsync(id);
            if (grupo == null)
            {
                throw new KeyNotFoundException("Grupo de permisos no encontrado");
            }
            
            var todosLosPermisos = (await _permisoRepo.GetAllPermisosAsync()).ToList();

            // El constructor del VM se encarga de rellenar y marcar los checkboxes
            var vm = new ActualizarGrupoViewModel(grupo, todosLosPermisos); 
            return vm;
        }

        public async Task RepoblarViewModelParaErrorAsync(CrearGrupoViewModel vm)
        {
            var todosLosPermisos = await _permisoRepo.GetAllPermisosAsync();
            var permisosSeleccionadosIds = new HashSet<int>(vm.PermisosSeleccionados);

            vm.TodosLosPermisos = todosLosPermisos.Select(p => new PermisoAsignadoViewModel
            {
                IdPermiso = p.IdPermiso,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                Asignado = permisosSeleccionadosIds.Contains(p.IdPermiso) // Re-marca los seleccionados
            }).ToList();
        }

        public async Task RepoblarViewModelParaErrorAsync(ActualizarGrupoViewModel vm)
        {
            var todosLosPermisos = (await _permisoRepo.GetAllPermisosAsync()).ToList();
            var permisosSeleccionadosIds = new HashSet<int>(vm.PermisosSeleccionados);

            vm.TodosLosPermisos = todosLosPermisos.Select(p => new PermisoAsignadoViewModel
            {
                IdPermiso = p.IdPermiso,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                Asignado = permisosSeleccionadosIds.Contains(p.IdPermiso) // Re-marca los seleccionados
            }).ToList();
        }
    }
}