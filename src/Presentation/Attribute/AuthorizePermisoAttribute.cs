using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json;

namespace src.Presentation.Attributes 
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class AuthorizePermisoAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string _permisoRequerido;
        private const string PERMISO_ADMIN = "P01_ADMINISTRADOR"; 

        public AuthorizePermisoAttribute(string permisoRequerido)
        {
            _permisoRequerido = permisoRequerido;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var usuarioId = context.HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(usuarioId))
            {
                context.Result = new RedirectToActionResult("Login", "Acceso", null);
                return;
            }

            var permisosJson = context.HttpContext.Session.GetString("Permisos");
            if (string.IsNullOrEmpty(permisosJson))
            {
                context.Result = new RedirectToActionResult("AccesoDenegado", "Home", null);
                return;
            }

            var permisos = JsonSerializer.Deserialize<List<string>>(permisosJson);
            
            if (permisos == null)
            {
                context.Result = new RedirectToActionResult("AccesoDenegado", "Home", null);
                return;
            }
           
            if (permisos.Contains(PERMISO_ADMIN))
            {
                return; 
            }
            if (!permisos.Contains(_permisoRequerido))
            {
                context.Result = new RedirectToActionResult("AccesoDenegado", "Home", null);
            }
        }
    }
}