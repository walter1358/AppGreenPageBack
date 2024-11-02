using GreenPageAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;


namespace GreenPageAPI.Controllers
{
    public class AuthController : ControllerBase
    {
        private readonly GreenPageContext dataContext;

        public AuthController(GreenPageContext context)
        {
            dataContext = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            //var user = await dataContext.Usuarios.FirstOrDefaultAsync(u => u.Login == model.User && u.Pass == model.Pass);
            var user = await dataContext.Usuarios.FirstOrDefaultAsync(u => u.Login == model.User);            if (user == null)
            if (user == null)
            
            {
                return NotFound("Usuario no encontrado.");
            }

            var response = new
            {
                message = "Login exitoso",
                userlogger = new 
                {
                    id = user.IdUsuario,
                    nomUsuario = user.NomUsuario,
                    idPerfil = user.IdPerfil,
                    perfilNombre = user.IdPerfil == 1 ? "Subastador/Ofertador" : "Admin"
                }
            };
            


            //return Ok(new { message = "Login exitoso" });
            return Ok(response);        }
        
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Usuario usuario)
        {
            // Verifica si el Login ya existe
            var existingUser = await dataContext.Usuarios
                .FirstOrDefaultAsync(u => u.Login == usuario.Login);
        // Validar los parámetros antes de llamar al SP
            if (existingUser != null)
            {
                return BadRequest("El usuario ya existe con ese Login.");
            }
            if (string.IsNullOrWhiteSpace(usuario.Login))
            {
                return BadRequest("El parámetro 'Login' no puede estar vacío.");
            }
            if (string.IsNullOrWhiteSpace(usuario.Pass))
            {
                return BadRequest("El parámetro 'Pass' no puede estar vacío.");
            }            
            if (string.IsNullOrWhiteSpace(usuario.NomUsuario))
            {
                return BadRequest("El parámetro 'NomUsuario' no puede estar vacío.");
            }
            if (string.IsNullOrWhiteSpace(usuario.Pregunta))
            {
                return BadRequest("El parámetro 'Pregunta' no puede estar vacío.");
            }
            if (string.IsNullOrWhiteSpace(usuario.Respuesta))
            {
                return BadRequest("El parámetro 'Respuesta' no puede estar vacío.");
            }            

            // Validar la contraseña
            if (!usuario.IsValidPassword())
            {
                return BadRequest("La contraseña debe tener al menos 1 mayúscula, 1 minúscula, 1 número y más de 8 caracteres.");
            }
        if (!usuario.ValidarEspaciosIzquierda(usuario.NomUsuario))
        {
            return BadRequest("El nombre de usuario no puede tener  espacios a la izquierda");
        }            
        // Validar el nombre de usuario
        if (!usuario.IsValidLength(usuario.NomUsuario.Trim()))
        {
            return BadRequest("La longitud del nombre de usuario es inválida: mayor a 10 y menor a 40, asegurate de no tener espacios en blanco");
        }        
        // Validar el nombre de usuario
        if (!usuario.ContainsOnlyLetters(usuario.NomUsuario))
        {
            return BadRequest("El nombre de usuario no puede contener números ni carácteres espaciales");
        }  
   
        // Hashea la contraseña del usuario
            usuario.Pass = BCrypt.Net.BCrypt.HashPassword(usuario.Pass);
            
            // Establecer el perfil como 1 por defecto
            usuario.IdPerfil = 1;

            // Establecer la fecha de creación a la fecha actual
            usuario.FecCreacion = DateTime.Now;

            try
            {
                // Agrega el nuevo usuario al contexto
                dataContext.Usuarios.Add(usuario);

                // Guarda los cambios en la base de datos
                await dataContext.SaveChangesAsync();

                return Ok(new { message = "Registro exitoso" });
            }
            catch (Exception ex)
            {
                var innerException = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(innerException);
            }
        }





}
        public class LoginModel
        {
            [Required]
            public string User { get; set; }

            [Required]
            public string Pass { get; set; }
        }

        

}
