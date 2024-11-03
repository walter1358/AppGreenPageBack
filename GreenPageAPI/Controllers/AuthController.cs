using GreenPageAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;



namespace GreenPageAPI.Controllers
{
    public class AuthController : ControllerBase
    {
        private readonly GreenPageContext dataContext;

        public AuthController(GreenPageContext context)
        {
            dataContext = context;
        }

        [HttpPost("recuperauser")]
        public async Task<IActionResult> Recuperauser([FromBody] UserModel usermodel)
        {
            var user = await dataContext.Usuarios.FirstOrDefaultAsync(u => u.Login == usermodel.User);
            if (user == null)
            {
                return NotFound("Usuario no encontrado.");
            }

            var response = new
            {
                message = "Usuario encontrado",
                userlogger = new 
                {
                    id = user.IdUsuario,
                    Pregunta = user.Pregunta
                    //nomUsuario = user.NomUsuario,
                    //idPerfil = user.IdPerfil,
                    //perfilNombre = user.IdPerfil == 1 ? "Subastador/Ofertador" : "Admin"
                }
            };            
            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            //var user = await dataContext.Usuarios.FirstOrDefaultAsync(u => u.Login == model.User && u.Pass == model.Pass);
            var user = await dataContext.Usuarios.FirstOrDefaultAsync(u => u.Login == model.User);
            if (user == null)
            
            {
                return NotFound("Usuario no encontrado.");
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(model.Pass, user.Pass);
            if (!isPasswordValid)
            {
                return Unauthorized("Contraseña incorrecta.");
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
            return Ok(response);
        }
        
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
                return BadRequest("La longitud del nombre de usuario es inválida: mayor a 4 y menor a 40, asegurate de no tener espacios en blanco");
            }        
            // Validar el nombre de usuario
            if (!usuario.ContainsOnlyLetters(usuario.NomUsuario))
            {
                return BadRequest("El nombre de usuario no puede contener números ni carácteres espaciales");
            }  

            if (!usuario.ValidarEspaciosIzquierda(usuario.Login))
            {
                return BadRequest("El Login no puede tener  espacios a la izquierda");
            }            
            // Validar el nombre de usuario
            if (!usuario.IsValidLength(usuario.Login.Trim()))
            {
                return BadRequest("La longitud del Login es inválida:debe ser mayor a 10 y menor a 40, asegurate de no tener espacios en blanco");
            }      

            if (!usuario.IsValidEmail(usuario.Login))
            {
                return BadRequest("El correo electrónico ingresado no es válido.");
            }            
    
            if (!usuario.ValidarEspaciosIzquierda(usuario.Respuesta))
            {
                return BadRequest("La respuesta no puede tener  espacios a la izquierda");
            }  

            if (!usuario.IsValidLengthRespuesta())
            {
                return BadRequest("La longitud de la Respuesta es inválida:debe ser mayor a 10 y menor a 40, asegurate de no tener espacios en blanco");
            } 
            // Validar el nombre de usuario
            // Validar el nombre de usuario
            if (!usuario.ContainsOnlyLetters(usuario.Respuesta))
            {
                return BadRequest("La respuesta no puede contener números ni carácteres espaciales");
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


            [HttpPost("actualizapass")]
            public async Task<IActionResult> ActualizaPass([FromBody] PassModel passModel)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }   
                
                // Verificar que Respuesta y Pass no estén vacíos
                if (string.IsNullOrWhiteSpace(passModel.Respuesta))
                {
                    return BadRequest("La respuesta no puede estar vacía.");
                }

                if (string.IsNullOrWhiteSpace(passModel.Pass))
                {
                    return BadRequest("La contraseña no puede estar vacía.");
                }

                //Buscamos al usuario por Login
                var user = await dataContext.Usuarios.FirstOrDefaultAsync(u =>
                u.Login == passModel.User && 
                u.Pregunta == passModel.Pregunta &&
                u.Respuesta == passModel.Respuesta);

                if (user == null)
                {
                    return NotFound("Usuario no encontrado o la información no coincide.");
                }

                // Validar la contraseña
                var passwordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$";
                if (!Regex.IsMatch(passModel.Pass, passwordPattern))
                {
                    return BadRequest("La contraseña debe contener al menos una letra mayúscula, una letra minúscula y un número.");
                }


                //Actualizmaos el pass
                //user.Pass = PassModel.Nuevopass;
                user.Pass = BCrypt.Net.BCrypt.HashPassword(passModel.Pass);


                await dataContext.SaveChangesAsync();

                return Ok(new { message = "Contraseña actualizada correctamente" });
            }



}
        public class LoginModel
        {
            [Required]
            public string User { get; set; }

            [Required]
            public string Pass { get; set; }
        }


        public class UserModel
        {
            [Required]
            public string User { get; set; }
        }        

        public class PassModel
        {
            [Required]
            public string User { get; set; }     

            [Required]
            public string Pregunta { get; set; }     

            [Required]
            public string Respuesta { get; set; }    

            [Required]
            public string Pass {get;set;}                                    
        }

}
