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
            // Verificar si el modelo es válido
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Devuelve los errores de validación
            }

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
            if (!ModelState.IsValid) return BadRequest(ModelState); // Devuelve los errores de validación

            try
            {
                var user = await GetUserByLoginAsync(model.User);
                if (user == null) return NotFound("Usuario no encontrado.");

                if (!IsUserActive(user)) return Unauthorized("Usuario inactivo.");

                if (!IsPasswordValid(model.Pass, user.Pass)) return Unauthorized("Contraseña incorrecta.");

                var response = GenerateLoginResponse(user);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        private async Task<Usuario?> GetUserByLoginAsync(string login)
        {
            return await dataContext.Usuarios.FirstOrDefaultAsync(u => u.Login == login);
        }

         static bool IsUserActive(Usuario user)
        {
            return user.isactive == true;
        }

        private static bool IsPasswordValid(string enteredPassword, string storedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(enteredPassword, storedPassword);
        }

        private object GenerateLoginResponse(Usuario user)
        {
            return new
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
        }

        private IActionResult HandleException(Exception ex)
        {
            // Opcional: verificar si es un error de conexión
            if (ex.InnerException != null && ex.InnerException.Message.Contains("connection"))
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "No se pudo conectar a la base de datos.");
            }

            return StatusCode(StatusCodes.Status500InternalServerError, "Ocurrió un error en el servidor.");
        }
      

        
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Usuario usuario)
        {

            

            //validamos usuario
            if (string.IsNullOrWhiteSpace(usuario.NomUsuario) && 
                string.IsNullOrWhiteSpace(usuario.ApeUsuario) && 
                string.IsNullOrWhiteSpace(usuario.dni) && 
                string.IsNullOrWhiteSpace(usuario.Login) && 
                string.IsNullOrWhiteSpace(usuario.Pass) && 
                string.IsNullOrWhiteSpace(usuario.Pregunta) && 
                string.IsNullOrWhiteSpace(usuario.Respuesta))
            {
                return BadRequest("Todos los campos están vacíos");
            }

   
            //validamos usuario
            if (string.IsNullOrWhiteSpace(usuario.NomUsuario))
            {
                return BadRequest("El parámetro 'Nombre' no puede estar vacío.");
            }
            if (!usuario.ValidarEspaciosIzquierda(usuario.NomUsuario))
            {
                return BadRequest("El nombre de usuario no puede tener  espacios a la izquierda");
            }    
            if (!usuario.IsValidLength(usuario.NomUsuario.Trim()))
            {
                return BadRequest("La longitud del nombre de usuario es inválida: mayor a 2 y menor a 40, asegurate de no tener espacios en blanco");
            }        
            if (!usuario.ContainsOnlyLetters(usuario.NomUsuario))
            {
                return BadRequest("El nombre de usuario no puede contener números ni carácteres espaciales");
            }       

            //validamos apellido
            if (string.IsNullOrWhiteSpace(usuario.ApeUsuario))
            {
                return BadRequest("El parámetro 'Apellido' no puede estar vacío.");
            }
            if (!usuario.ValidarEspaciosIzquierda(usuario.ApeUsuario))
            {
                return BadRequest("El Apellido de usuario no puede tener  espacios a la izquierda");
            }    
            if (!usuario.IsValidLength(usuario.ApeUsuario.Trim()))
            {
                return BadRequest("La longitud del Apellido de usuario es inválida: mayor a 2 y menor a 40, asegurate de no tener espacios en blanco");
            }        
            if (!usuario.ContainsOnlyLetters(usuario.ApeUsuario))
            {
                return BadRequest("El Apellido de usuario no puede contener números ni carácteres espaciales");
            } 

            // Verifica si el Login ya existe
            var existingUser = await dataContext.Usuarios.FirstOrDefaultAsync(u => u.Login == usuario.Login);
            if (existingUser != null)
            {
                return BadRequest("El usuario ya existe con ese Login.");
            }
            if (string.IsNullOrWhiteSpace(usuario.dni))
            {
                return BadRequest("El parámetro 'dni' no puede estar vacío.");
            }            
            if (usuario.dni.Length != 8)
            {
                return BadRequest("El parámetro 'dni' debe tener 8 digitos.");
            }
            if (!usuario.dni.All(char.IsDigit))
            {
                return BadRequest("El parámetro 'dni' solo debe tener dígitos numéricos.");
            }            

            // Verifica si el DNI ya existe
            var existingdni = await dataContext.Usuarios.FirstOrDefaultAsync(u => u.dni == usuario.dni);
            if (existingdni != null)
                {
                    return BadRequest("El usuario ya existe con ese dni.");
                }        


            //validamos el Login
            if (string.IsNullOrWhiteSpace(usuario.Login))
            {
                return BadRequest("El parámetro 'Login' no puede estar vacío.");
            }
            if (!usuario.ValidarEspaciosIzquierda(usuario.Login))
            {
                return BadRequest("El Login no puede tener  espacios a la izquierda");
            }            
            if (!usuario.IsValidLength(usuario.Login.Trim()))
            {
                return BadRequest("El correo electrónico ingresado para el login no es válido");
            }      
            if (!usuario.IsValidEmail(usuario.Login))
            {
                return BadRequest("El correo electrónico ingresado para el login no es válido.");
            }  

            // Validar la contraseña
            if (string.IsNullOrWhiteSpace(usuario.Pass))
            {
                return BadRequest("El parámetro 'Pass' no puede estar vacío.");
            }     
            if (!usuario.IsValidPassword())
            {
                return BadRequest("La contraseña debe tener al menos 1 mayúscula, 1 minúscula, 1 número y más de 8 caracteres.");
            }

            //validamos pregunta
            if (string.IsNullOrWhiteSpace(usuario.Pregunta))
            {
                return BadRequest("El parámetro 'Pregunta' no puede estar vacío.");
            }


            //validamos respuesta
            if (string.IsNullOrWhiteSpace(usuario.Respuesta))
            {
                return BadRequest("El parámetro 'Respuesta' no puede estar vacío.");
            }                               
            if (!usuario.ValidarEspaciosIzquierda(usuario.Respuesta))
            {
                return BadRequest("La respuesta no puede tener  espacios a la izquierda");
            }  
            if (!usuario.IsValidLengthRespuesta())
            {
                return BadRequest("La longitud de la Respuesta es inválida:debe ser menor a 40, asegurate de no tener espacios en blanco");
            } 

            //validamos respuesta
            if (!usuario.ContainsOnlyLetters(usuario.Respuesta))
            {
                return BadRequest("La respuesta no puede contener números ni carácteres espaciales");
            }        


           if (!ModelState.IsValid)
             {
               return BadRequest(ModelState);
             }   

          // Hashea la contraseña del usuario
            usuario.Pass = BCrypt.Net.BCrypt.HashPassword(usuario.Pass);
            
            // Establecer el perfil como 1 por defecto si es nulo
            if (usuario.IdPerfil == null || usuario.IdPerfil <= 0)
            {
                usuario.IdPerfil = 1;
            }

            usuario.isactive = true;

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
