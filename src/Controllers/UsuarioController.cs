using Microsoft.AspNetCore.Mvc;
using src.Repositories.Interfaces; 
using src.ViewModels;
using System.Linq; 
using Dom = src.Models.Domain;

namespace src.Controllers
{

    public class UsuarioController : Controller 
    {
        private readonly IUsuarioRepository _repoUsuario;

      
        public UsuarioController(IUsuarioRepository repoUsuario)
        {
            _repoUsuario = repoUsuario;
        }

        [HttpGet]
        public async Task<IActionResult> VerUsuario(int idUser)
        {
            return View(await _repoUsuario.GetUsuarioByIdAsync(idUser));
        }
        
        [HttpGet]
        public async Task<IActionResult> ListarUsuarios()
        {
            var listaUsuarios = await _repoUsuario.GetAllUsuarioAsync();
            var usuariosActivos = listaUsuarios.Where(u => u.Activo == true);
                                     
            return View("ListarUsuarios", usuariosActivos);
        }

        [HttpGet]
        public IActionResult CrearUsuario()
        {
            
            var viewModel = new CrearUsuarioViewModel();
            return View("CrearUsuario", viewModel); // Devuelve la vista del formulario
        }

        
        [HttpPost]
        public async Task<IActionResult> CrearUsuario([FromForm] CrearUsuarioViewModel usuarioVM)
        {
            // La contraseña es obligatoria solo al crear
            if (string.IsNullOrEmpty(usuarioVM.Contrasenia))
            {
                ModelState.AddModelError(nameof(usuarioVM.Contrasenia), "La contraseña es obligatoria");
            }

            if (!ModelState.IsValid)
            {
                // Si falla, solo devolvemos el VM, no necesitamos recargar listas.
                return View("CrearUsuario", usuarioVM);
            }

            // Usamos el método estático del VM para mapear (igual que tu patrón)
            Dom.Usuario usuario = CrearUsuarioViewModel.CargarUsuario(usuarioVM);
            
            await _repoUsuario.AddAsync(usuario);

            TempData["realizado"] = "El Usuario fue creado con éxito.";
            return RedirectToAction("ListarUsuarios");
        }

        [HttpGet]
        public async Task<IActionResult> ActualizarUsuario(int idUsuario) // Cambiado 'id' a 'idUsuario' por claridad
        {
            Dom.Usuario usuario = await _repoUsuario.GetUsuarioByIdAsync(idUsuario);
            if (usuario == null)
            {
                return NotFound();
            }

            // Usamos el constructor del ViewModel para rellenar el formulario
            var usuarioViewModel = new ActualizarUsuarioViewModel(usuario);

            // Reutilizamos la misma vista del formulario de creación
            return View(usuarioViewModel);
        }

        
        [HttpPost]
        public async Task<IActionResult> ActualizarUsuario([FromForm]   ActualizarUsuarioViewModel usuarioVM)
        {
            
            if (!ModelState.IsValid)
            {
                // Si falla, devolvemos la vista con el VM
                return View("ActualizarUsuario", usuarioVM);
            }

            // 1. Obtenemos el usuario existente de la BD
            Dom.Usuario? usuarioExistente = await _repoUsuario.GetUsuarioByIdAsync(usuarioVM.IdUsuario);
            if (usuarioExistente == null)
            {
                return NotFound();
            }

            // 2. Mapeamos manualmente solo los campos editables
            usuarioExistente.Nombre = usuarioVM.Nombre;
            usuarioExistente.Apellido = usuarioVM.Apellido;
            usuarioExistente.Identificacion = usuarioVM.Identificacion;
            usuarioExistente.Correo = usuarioVM.Correo;
            usuarioExistente.Telefono = usuarioVM.Telefono;
            
            if (!string.IsNullOrEmpty(usuarioVM.Contrasenia))
            {
            // falta HASHEAR la contraseña 
            usuarioExistente.Contrasenia = usuarioVM.Contrasenia; 
            }

            await _repoUsuario.UpdateAsync(usuarioExistente);

            TempData["realizado"] = "El Usuario fue actualizado con éxito.";
            return RedirectToAction("ListarUsuarios");
        }

       /*
        
    */
      
        [HttpGet]
        public async Task<IActionResult> EliminarUsuario(int idUsuario) 
        {
            await _repoUsuario.DeleteAsync(idUsuario); 
            TempData["realizado"] = "El Usuario fue desactivado con éxito.";
            return RedirectToAction("ListarUsuarios");
        }
    }
}












// using Microsoft.AspNetCore.Mvc;
// using src.Repositories.Interfaces;
// using Dom = src.Models.Domain;

// namespace src.Controllers;

// [ApiController]
// [Route("api/[controller]")]
// public class UsuariosController : ControllerBase
// {
//     private readonly IUsuarioRepository _repository;

//     public UsuariosController(IUsuarioRepository repository)
//     {
//         _repository = repository;
//     }

//     // GET: api/Usuarios
//     [HttpGet]
//     public async Task<IActionResult> GetAll()
//     {
//         var usuarios = await _repository.GetAllUsuarioAsync();
//         return Ok(usuarios);
//     }

//     // GET: api/Usuarios/5
//     [HttpGet("{id}")]
//     public async Task<IActionResult> GetById(int id)
//     {
//         var usuario = await _repository.GetUsuarioByIdAsync(id);
//         if (usuario == null)
//         {
//             return NotFound($"Usuario con ID {id} no encontrado.");
//         }
//         return Ok(usuario);
//     }

//     // POST: api/Usuarios
//     [HttpPost]
//     public async Task<IActionResult> Create([FromBody] Dom.Usuario usuario)
//     {
//         if (usuario == null || !ModelState.IsValid)
//         {
//             return BadRequest(ModelState);
//         }

//         await _repository.AddAsync(usuario);

//         // Devuelve el objeto creado y la URL para acceder a él (código 201)
//         return CreatedAtAction(nameof(GetById), new { id = usuario.IdUsuario }, usuario);
//     }

//     // PUT: api/Usuarios/5
//     [HttpPut("{id}")]
//     public async Task<IActionResult> Update(int id, [FromBody] Dom.Usuario usuario)
//     {
//         if (usuario == null || id != usuario.IdUsuario)
//         {
//             return BadRequest("El ID del usuario no coincide.");
//         }

//         if (!ModelState.IsValid)
//         {
//             return BadRequest(ModelState);
//         }

//         // Opcional: Verificar si el usuario existe antes de actualizar
//         var existingUser = await _repository.GetUsuarioByIdAsync(id);
//         if (existingUser == null)
//         {
//             return NotFound($"Usuario con ID {id} no encontrado.");
//         }

//         await _repository.UpdateAsync(usuario);

//         // Devuelve 204 No Content, indicando éxito sin devolver datos.
//         return NoContent();
//     }

//     // DELETE: api/Usuarios/5
//     [HttpDelete("{id}")]
//     public async Task<IActionResult> Delete(int id)
//     {
//         bool result = await _repository.DeleteAsync(id);

//         if (!result)
//         {
//             return NotFound($"Usuario con ID {id} no encontrado.");
//         }

//         // Devuelve 204 No Content
//         return NoContent();
//     }
// }