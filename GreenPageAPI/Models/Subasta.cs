using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GreenPageAPI.Models
{
    [Table("SUBASTA")]
    public class Subasta
    {
        [Key]
        public int IdSubasta { get; set; }

        [Required]
        public int IdLibro { get; set; }

        [Required]
        public DateTime FecInicio { get; set; }

        [Required]
        public DateTime FecFinal { get; set; }

        [Required]
        public double PrecioBase { get; set; }

        public int? IdUsuario { get; set; }
    }
}
