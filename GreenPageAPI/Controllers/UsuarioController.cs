using GreenPageAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;



namespace GreenPageAPI.Controllers
{

    [ApiController]
    [Route("api/[controller]")]    
    public class UsuarioController :  ControllerBase
    {

        private readonly GreenPageContext _context;

        public UsuarioController(GreenPageContext context)
        {
            _context = context;
        }

        // Método para actualizar el estado de isactive
        [HttpPut("cambiarEstado/{id}")]
        public async Task<IActionResult> CambiarEstado(int id, [FromBody] bool nuevoEstado)
        {
            // Buscar el usuario por Id
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound(new { message = "Usuario no encontrado" });
            }

            // Actualizar el estado de isactive
            usuario.isactive = nuevoEstado;
            _context.Entry(usuario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { message = "Estado actualizado correctamente" });
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, new { message = "Error al actualizar el estado" });
            }
        }        


        [HttpGet("Usuarios")]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            var usuarios = await _context.Usuarios
            .Select(u => new
            {
                u.IdUsuario,
                PerfilNombre = u.IdPerfil == 1 ? "Subastador/Ofertador" : "Administrador",
                u.IdPerfil,
                u.dni,
                u.NomUsuario,
                u.ApeUsuario,
                u.Login,
                u.Pass,
                u.FecCreacion,
                u.isactive
            })
            .ToListAsync();

        return Ok(usuarios);
        }

        [HttpPost("cambiarContrasena")]
        public async Task<IActionResult> CambiarContrasena([FromBody] UsuarioModel usuarioModel)
        {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }                   

                if (string.IsNullOrWhiteSpace(usuarioModel.Pass))
                {
                    return BadRequest("La contraseña no puede estar vacía.");
                }

                //Buscamos al usuario por Login
                var user = await _context.Usuarios.FirstOrDefaultAsync(u =>
                u.IdUsuario == usuarioModel.User);

                if (user == null)
                {
                    return NotFound("Usuario no encontrado o la información no coincide.");
                }

                // Validar la contraseña
                var passwordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$";
                if (!Regex.IsMatch(usuarioModel.Pass, passwordPattern))
                {
                    return BadRequest("La contraseña debe contener al menos una letra mayúscula, una letra minúscula y un número.");
                }

                //Actualizmaos el pass
                //user.Pass = PassModel.Nuevopass;
                user.Pass = BCrypt.Net.BCrypt.HashPassword(usuarioModel.Pass);


                await _context.SaveChangesAsync();

                return Ok(new { message = "Contraseña actualizada correctamente" });
        }        

        // Método para eliminar un usuario por Id
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarUsuario(int id)
        {
            // Buscar el usuario por Id
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound(new { message = "Usuario no encontrado" });
            }

            // Eliminar el usuario
            _context.Usuarios.Remove(usuario);

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { message = "Usuario eliminado correctamente" });
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, new { message = "Error al eliminar el usuario" });
            }
        }

        // Método para editar un usuario por Id
        [HttpPut("{id}")]
        public async Task<IActionResult> EditarUsuario(int id, [FromBody] UsuarioUpdt usuarioActualizado)
        {
            // Buscar el usuario por Id
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound(new { message = "Usuario no encontrado" });
            }

            // Actualizar los campos permitidos
            usuario.IdPerfil = usuarioActualizado.idPerfil;
            usuario.NomUsuario = usuarioActualizado.nomUsuario;
            usuario.ApeUsuario = usuarioActualizado.apeUsuario;
            usuario.dni = usuarioActualizado.dni;
            usuario.Login = usuarioActualizado.login;

            // Marcar el usuario como modificado
            _context.Entry(usuario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { message = "Usuario actualizado correctamente" });
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, new { message = "Error al actualizar el usuario" });
            }
        }



    }

    public class UsuarioUpdt
    {
        public int idPerfil {get;set;}
        public string nomUsuario {get;set;}
        public string apeUsuario {get;set;}
        public string dni{get;set;}
        public string login{get;set;}

    }

    public class UsuarioModel
    {
        [Required]
        public int User { get; set; }      

        [Required]
        public string Pass {get;set;}                                    
    }   

}