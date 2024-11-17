using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;


namespace GreenPageAPI.Models
{
    [Table("USUARIO")]
    public class Usuario
    {
        [Key]
        public int? IdUsuario { get; set; }
        public int? IdPerfil { get; set; }

        [Required]
        [StringLength(40)]
        public string NomUsuario { get; set; }

        [Required]
        [StringLength(40)]
        public string ApeUsuario  {get;set;}

        [Required]
        [StringLength(8)]
        public string dni  {get;set;}

        [Required]
        [StringLength(40)]
        public string Login { get; set; }

        [Required]
        [StringLength(60)]
        public string Pass { get; set; }

        [StringLength(60)]
        public string? Pregunta { get; set; } // Opcional, puedes añadir [Required] si es necesario.

        [StringLength(60)]
        public string? Respuesta { get; set; } // Opcional, puedes añadir [Required] si es necesario.                

        public DateTime? FecCreacion { get; set; }

        public bool? isactive{get;set;}

        public bool IsValidPassword()
        {
            // Validar que la contraseña tenga al menos una letra mayúscula, una minúscula, un número y más de 8 caracteres
            return !string.IsNullOrEmpty(Pass) &&
                   Pass.Length > 8 &&
                   Pass.Any(char.IsUpper) && // Al menos una mayúscula
                   Pass.Any(char.IsLower) && // Al menos una minúscula
                   Pass.Any(char.IsDigit);    // Al menos un número
        } 

        public bool IsValidLength(string username)
        {
            return username.Length >= 2 && username.Length <= 40;
        }
        public bool IsValidLengthRespuesta()
        {
            return Respuesta.Trim().Length <= 40;
        }        

        public bool ContainsOnlyLetters(string username)
        {
    // Permite letras (mayúsculas y minúsculas) y espacios
            return username.All(c => char.IsLetter(c) || char.IsWhiteSpace(c));            
        }   
          
        public  bool ValidarEspaciosIzquierda(string cadena)
        {
                // Establecer el límite de espacios a 2 en la expresión regular para ambos lados
                string pattern = @"^ {0,0}[^ ](.*?[^ ].*?)? {0,2}$";
                Regex regex = new Regex(pattern);
                
                return regex.IsMatch(cadena);
        }   

    // Método para validar el formato del correo electrónico
    public bool IsValidEmail(string email)
    {
        // Expresión regular para validar el formato del correo electrónico
        string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, pattern);
    }                  

}
}