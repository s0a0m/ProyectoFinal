using Microsoft.AspNetCore.Mvc;
using src.Core.Services.Interfaces;
using src.Presentation.Attributes;
using Dom = src.Models.Domain;
using src.Presentation.ViewModels.UsuarioVM;
using Microsoft.AspNetCore.Session;
using Microsoft.AspNetCore.Http;

namespace src.Controllers
{

    public class UsuarioController : Controller
    {
        private readonly IUserService _usuarioService;

        public UsuarioController(IUserService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpGet]
        [AuthorizePermiso("P02_VER_LISTA_USUARIOS")]
        public async Task<IActionResult> VerUsuario(int idUser)
        {
            try
            {
                Dom.Usuario usuario = await _usuarioService.GetUserByIdAsync(idUser);
                return View(usuario);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet]
        [AuthorizePermiso("P02_VER_LISTA_USUARIOS")]
        public async Task<IActionResult> ListarUsuarios()
        {
            var usuariosActivos = await _usuarioService.GetUsersAsync();
            return View("ListarUsuarios", usuariosActivos);
        }

        [HttpGet]
        [AuthorizePermiso("P10_ABM_USUARIOS")]
        public async Task<IActionResult> CrearUsuario()
        {

            var viewModel = await _usuarioService.PrepararCrearViewModelAsync();
            return View("CrearUsuario", viewModel);
        }


        [HttpPost]
        [AuthorizePermiso("P10_ABM_USUARIOSR")]
        public async Task<IActionResult> CrearUsuario([FromForm] CrearUsuarioViewModel usuarioVM)
        {
            if (!ModelState.IsValid)
            {
                await _usuarioService.RepoblarViewModelParaErrorAsync(usuarioVM);
                return View("CrearUsuario", usuarioVM);
            }

            try
            {
                Dom.Usuario nuevoUsuario = await _usuarioService.CreateUserAsync(usuarioVM);
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(ex.ParamName ?? string.Empty, ex.Message);
                await _usuarioService.RepoblarViewModelParaErrorAsync(usuarioVM);
                return View("CrearUsuario", usuarioVM);
            }

            TempData["realizado"] = "El Usuario fue creado con éxito.";
            return RedirectToAction("ListarUsuarios");
        }

        [HttpGet]
         [AuthorizePermiso("P10_ABM_USUARIOS")]
        public async Task<IActionResult> ActualizarUsuario(int idUsuario)
        {
            Dom.Usuario usuario;

            try
            {
                var usuarioViewModel = await _usuarioService.PrepararActualizarViewModelAsync(idUsuario);
                return View("ActualizarUsuario", usuarioViewModel);

            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }


        [HttpPost]
        [AuthorizePermiso("P10_ABM_USUARIOS")]
        public async Task<IActionResult> ActualizarUsuario([FromForm] ActualizarUsuarioViewModel usuarioVM)
        {
            if (!ModelState.IsValid)
            {
                await _usuarioService.RepoblarViewModelParaErrorAsync(usuarioVM);
                return View("ActualizarUsuario", usuarioVM);
            }

            try
            {
                await _usuarioService.UpdateUserAsync(usuarioVM);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (ArgumentException ex) // Para validaciones de negocio futuras (ej. email duplicado)
            {
                ModelState.AddModelError(ex.ParamName ?? string.Empty, ex.Message);
                await _usuarioService.RepoblarViewModelParaErrorAsync(usuarioVM);
                return View("ActualizarUsuario", usuarioVM);
            }

            TempData["realizado"] = "El Usuario fue actualizado con éxito.";
            return RedirectToAction("ListarUsuarios");
        }

        [HttpPost]
        [AuthorizePermiso("P10_ABM_USUARIOS")]
        public async Task<IActionResult> EliminarUsuario(int idUsuario)
        {
            try
            {
                await _usuarioService.DisableUserAsync(idUsuario);
                TempData["realizado"] = "El Usuario fue desactivado con éxito.";
                return RedirectToAction("ListarUsuarios");
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        [AuthorizePermiso("P10_ABM_USUARIOS")]
        public async Task<IActionResult> ReactivarUsuario(int idUsuario)
        {
            try
            {
                await _usuarioService.ReactivarUsuarioAsync(idUsuario); 
                TempData["realizado"] = "El usuario ha sido reactivado y sus permisos restaurados.";
            }
            catch (KeyNotFoundException)
            {
                TempData["error"] = "El usuario que intentas reactivar no existe.";
            }
            catch (Exception ex)
            {
                TempData["error"] = "Ocurrió un error al reactivar: " + ex.Message;
            }

            return RedirectToAction("ListarUsuarios");
        }
    }
}