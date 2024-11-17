using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GreenPageAPI.Models
{
    [Table("LIBRO")]
    public class Libro
    {
        [Key]
        public int? IdLibro { get; set; }

        [Required]
        public int? IdEditorial { get; set; }

        [Required]
        public int? IdGenero { get; set; }

        
        [ForeignKey("IdEditorial")]
        public Editorial Editorial { get; set; } // Propiedad de navegación a Editorial

        [ForeignKey("IdGenero")]
        public Genero Genero { get; set; } // Propiedad de navegación a Genero        

        [Required]
        [StringLength(255)]
        public string Destitulo { get; set; }

        [Required]
        [StringLength(50)]
        public string Estado { get; set; }

        [Required]
        [StringLength(20)]
        public string ISBN { get; set; }

        [Required]
        [StringLength(1000)]
        public string Sinopsys { get; set; }

        public int? IdUsuario { get; set; }
    }


    [Table("EDITORIAL")]
    public class Editorial
    {
        [Key]
        public int? ideditorial { get; set; }
        public string deseditorial { get; set; }
    }

    [Table("GENERO")]
    public class Genero
    {
        [Key]
        public int? idgenero { get; set; }
        public string desgenero { get; set; }
    }

}
