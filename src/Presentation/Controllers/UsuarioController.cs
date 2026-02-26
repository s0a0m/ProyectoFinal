using Microsoft.AspNetCore.Mvc;
using src.Core.Services.Interfaces;
using src.Presentation.Attributes;
using Dom = src.Models.Domain;
using src.Presentation.ViewModels.UsuarioVM;
using Microsoft.AspNetCore.Session;
using Microsoft.AspNetCore.Http;
using src.Presentation.Controllers;

namespace src.Controllers
{

    public class UsuarioController : BaseController
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
        [ValidateAntiForgeryToken]
        [AuthorizePermiso("P10_ABM_USUARIOS")]
        public async Task<IActionResult> CrearUsuario([FromForm] CrearUsuarioViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await _usuarioService.RepoblarViewModelParaErrorAsync(model);
                return View(model);
            }

            var result = await _usuarioService.CreateUserAsync(model);

            if (!result.Success)
            {
                MapServiceErrors(result);

                await _usuarioService.RepoblarViewModelParaErrorAsync(model);

                return View(model);
            }

            SetSuccessMessage(result.Message);

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

            var result = await _usuarioService.UpdateUserAsync(usuarioVM);

            if (!result.Success)
            {
                MapServiceErrors(result);
                await _usuarioService.RepoblarViewModelParaErrorAsync(usuarioVM);
                return RedirectToAction("ActualizarUsuario", usuarioVM);
            }

            SetSuccessMessage(result.Message);
            return RedirectToAction("ListarUsuarios");
        }

        [HttpPost]
        [AuthorizePermiso("P10_ABM_USUARIOS")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarUsuario(int idUsuario)
        {
            var result = await _usuarioService.DisableUserAsync(idUsuario);

            if (!result.Success)
            {
                MapServiceErrors(result);
                return RedirectToAction("ListarUsuarios");
            }
            SetSuccessMessage(result.Message);

            return RedirectToAction("ListarUsuarios");
        }

        [HttpPost]
        [AuthorizePermiso("P10_ABM_USUARIOS")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReactivarUsuario(int idUsuario)
        {
            var result = await _usuarioService.ReactivarUsuarioAsync(idUsuario);

            if (!result.Success)
            {
                MapServiceErrors(result);
                return RedirectToAction("ListarUsuarios");
            }

            SetSuccessMessage(result.Message);

            return RedirectToAction("ListarUsuarios");
        }
    }
}