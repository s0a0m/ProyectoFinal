using Dom = src.Models.Domain;

namespace src.Repositories.Interfaces;

public interface IGrupoPermisosRepository
{
    Task<IEnumerable<Dom.GrupoPermisos>> GetAllAsync();
    Task<Dom.GrupoPermisos?> GetByIdAsync(short id);
    Task<bool> AddPermisoToGrupo(short idGrupo, int idPermiso);
    Task<bool> RemovePermisoFromGrupo(short idGrupo, int idPermiso);
    Task UpdateAsync(Dom.GrupoPermisos domainEntity);
    Task<bool> DeleteAsync(int id);

    // Task<bool> AddGrupoToUsuario(short idGrupo, short idUsuario);
    // Task<bool> RemoveGrupoFromUsuario(short idGrupo, short idUsuario);
    // Task<IEnumerable<Dom.GrupoPermisos>> GetGruposByUsuarioIdAsync(short idUsuario);
}