using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GreenPageAPI.Models
{
    [Table("SUBASTA")]
    public class Subasta
    {
        [Key]
        public int? IdSubasta { get; set; }

        [Required]
        public int? IdLibro { get; set; }

        [ForeignKey("IdLibro")]
        public Libro Libro { get; set; } // Propiedad de navegación hacia Libro        

        [Required]
        public DateTime? FecInicio { get; set; }

        [Required]
        public DateTime? FecFinal { get; set; }

        [Required]
        public double PrecioBase { get; set; }

        public int? IdUsuario { get; set; }

        public DateTime? StartTime { get; set; }
        public int? Duration { get; set; }  // Duración en segundos
        public DateTime? EndTime { get; set; }
        public bool? isclosed { get; set; }        
    



    }
}
