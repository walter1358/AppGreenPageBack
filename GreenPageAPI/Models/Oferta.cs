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
        public int? IdSubasta { get; set; }

        [ForeignKey("IdUsuario")]
        public Usuario? usuario { get; set; } // Propiedad de navegación hacia el usuario       

        [ForeignKey("IdSubasta")]
        public Subasta? subasta {get;set;} //propuedad de navegación hacia la subasta( para obtener el nombre del libro)


        [Required]
        public double PrecioOferta { get; set; }

        public DateTime? FecOferta { get; set; }

        public int? IdUsuario { get; set; }
    }
}
