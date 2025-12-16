using Microsoft.AspNetCore.Http;
using src.Presentation.Extensions; // Donde está tu SessionExtensions

namespace src.Presentation.Services
{
    public class LayoutService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LayoutService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public bool TienePermiso(string nombrePermiso)
        {
            return _httpContextAccessor.HttpContext?.Session.TienePermiso(nombrePermiso) ?? false;
        }

        public bool EstaLogueado()
        {
             return !string.IsNullOrEmpty(_httpContextAccessor.HttpContext?.Session.GetString("UsuarioId"));
        }
        
        public string ObtenerNombreUsuario()
        {
             return _httpContextAccessor.HttpContext?.Session.GetString("Nombre") ?? "Usuario";
        }
    }
}