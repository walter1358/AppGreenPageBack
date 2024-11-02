using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GreenPageAPI.Models
{
    [Table("OFERTA")]
    public class Oferta
    {
        [Key]
        public int IdOferta { get; set; }

        [Required]
        public int IdSubasta { get; set; }

        [Required]
        public double PrecioOferta { get; set; }

        public DateTime? FecOferta { get; set; }

        public int? IdUsuario { get; set; }
    }
}
