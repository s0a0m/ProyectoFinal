using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace src.Presentation.Extensions 
{
    public static class SessionExtensions
    {
        private const string PERMISO_ADMIN = "P01_ADMINISTRADOR";

        public static bool TienePermiso(this ISession session, string permisoRequerido)
        {
            var permisosJson = session.GetString("Permisos");
            if (string.IsNullOrEmpty(permisosJson)) return false;

            try 
            {
                var lista = JsonSerializer.Deserialize<List<string>>(permisosJson);
                
                if (lista == null) return false;

                // --- LÓGICA MODIFICADA ---
                
                // 1. Si es Admin, tiene permiso para TODO (retorna true)
                if (lista.Contains(PERMISO_ADMIN)) return true;

                // 2. Si no es Admin, buscamos el permiso específico
                return lista.Contains(permisoRequerido);
            }
            catch 
            {
                return false;
            }
        }
    }
}