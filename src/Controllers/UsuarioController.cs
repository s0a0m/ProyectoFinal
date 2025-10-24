using Microsoft.AspNetCore.Mvc;
using src.Repositories.Interfaces;
using Dom = src.Models.Domain;

namespace src.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioRepository _repository;

    public UsuariosController(IUsuarioRepository repository)
    {
        _repository = repository;
    }

    // GET: api/Usuarios
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var usuarios = await _repository.GetAllUsuarioAsync();
        return Ok(usuarios);
    }

    // GET: api/Usuarios/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var usuario = await _repository.GetUsuarioByIdAsync(id);
        if (usuario == null || usuario.Activo == false)
        {
            return NotFound($"Usuario con ID {id} no encontrado.");
        }
        return Ok(usuario);
    }

    // POST: api/Usuarios
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Dom.Usuario usuario)
    {
        if (usuario == null || !ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await _repository.AddAsync(usuario);

        // Devuelve el objeto creado y la URL para acceder a él (código 201)
        return CreatedAtAction(nameof(GetById), new { id = usuario.IdUsuario }, usuario);
    }

    // PUT: api/Usuarios/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Dom.Usuario usuario)
    {
        if (usuario == null || id != usuario.IdUsuario)
        {
            return BadRequest("El ID del usuario no coincide.");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Opcional: Verificar si el usuario existe antes de actualizar
        var existingUser = await _repository.GetUsuarioByIdAsync(id);
        if (existingUser == null)
        {
            return NotFound($"Usuario con ID {id} no encontrado.");
        }

        await _repository.UpdateAsync(usuario);

        // Devuelve 204 No Content, indicando éxito sin devolver datos.
        return NoContent();
    }

    // DELETE: api/Usuarios/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        bool result = await _repository.DeleteAsync(id);

        if (!result)
        {
            return NotFound($"Usuario con ID {id} no encontrado.");
        }

        // Devuelve 204 No Content
        return NoContent();
    }
}