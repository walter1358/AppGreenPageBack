using Microsoft.EntityFrameworkCore;

namespace GreenPageAPI.Models
{
    public class GreenPageContext : DbContext
    {
        public GreenPageContext(DbContextOptions<GreenPageContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios {get; set;}
        public DbSet<Libro> Libros { get; set; }
        public DbSet<Subasta> Subastas { get; set; }
        public DbSet<Oferta> Ofertas { get; set; }

        public DbSet<Editorial> Editoriales {get;set;}
        public DbSet<Genero> Generos {get;set;}
        

    }
}
