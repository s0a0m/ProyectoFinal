using Microsoft.AspNetCore.Mvc;
using src.Core.Services.Interfaces;
using Presentation.ViewModels.AccesoVM;
using System.Text.Json; 

namespace src.Controllers
{
    public class AccesoController : Controller
    {
        private readonly IUserService _usuarioService;

        public AccesoController(IUserService usuarioService)
        {
            _usuarioService = usuarioService;
        }

       
        [HttpGet]
        public IActionResult Login()
        {
            // Si ya está logueado, mandarlo al Home
            if (HttpContext.Session.GetString("UsuarioId") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (!ModelState.IsValid) return View(model);

            // 1. Llamamos al servicio (que ya aplana los permisos y valida pass)
            var usuario = await _usuarioService.ValidarUsuario(model.Correo, model.Clave);
            if (usuario != null  )
            {
                if (usuario.Activo is false)
                {
                    ViewBag.Error = "Acceso Denegado";
                    return View();
                }
                // 2. Crear la Sesión
                HttpContext.Session.SetString("UsuarioId", usuario.IdUsuario.ToString());
                HttpContext.Session.SetString("Nombre", usuario.Nombre);
                HttpContext.Session.SetString("Correo", usuario.Correo);

                var listaPermisos = usuario.PermisosUsuario.Select(p => p.Nombre).ToList();
                string permisosJson = JsonSerializer.Serialize(listaPermisos);
                
                HttpContext.Session.SetString("Permisos", permisosJson);

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Correo o contraseña incorrectos";
            return View(model);
        }

        public IActionResult Logout()
        {
            // Borrar sesión
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}