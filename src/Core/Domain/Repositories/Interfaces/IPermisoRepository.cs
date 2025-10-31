using EF = src.Models.CodeFirst;
using Dom = src.Models.Domain;
namespace src.Repositories.Interfaces;

public interface IPermisoRepository
{
    Task<IEnumerable<Dom.Permiso>> GetAllPermisosAsync();
    Task<Dom.Permiso?> GetPermisoById(int id);
    Task<bool> AsignarPermisoAUsuario(int idPermiso, int idUsuario);
    Task<bool> AsignarPermisosAUsuario(IEnumerable<int> idsPermisos, int idUsuario);
    Task<IEnumerable<Dom.Permiso>> GetPermisosByUsuarioIdAsync(int idUsuario);
    Task ReemplazarPermisosAsync(int idUsuario, IEnumerable<int> nuevosIdsPermisos);
}
