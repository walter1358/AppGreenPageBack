using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GreenPageAPI.Models
{
    [Table("USUARIO")]
    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }
        public int? IdPerfil { get; set; }

        [Required]
        [StringLength(255)]
        public string NomUsuario { get; set; }

        [Required]
        [StringLength(100)]
        public string Login { get; set; }

        [Required]
        [StringLength(100)]
        public string Pass { get; set; }

        [StringLength(255)]
        public string? Pregunta { get; set; } // Opcional, puedes añadir [Required] si es necesario.

        [StringLength(255)]
        public string? Respuesta { get; set; } // Opcional, puedes añadir [Required] si es necesario.                

        public DateTime? FecCreacion { get; set; }

        public bool IsValidPassword()
        {
            // Validar que la contraseña tenga al menos una letra mayúscula, una minúscula, un número y más de 8 caracteres
            return !string.IsNullOrEmpty(Pass) &&
                   Pass.Length > 8 &&
                   Pass.Any(char.IsUpper) && // Al menos una mayúscula
                   Pass.Any(char.IsLower) && // Al menos una minúscula
                   Pass.Any(char.IsDigit);    // Al menos un número
        }        
    }
}
