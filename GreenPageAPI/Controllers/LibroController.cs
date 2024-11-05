using GreenPageAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GreenPageAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LibroController :  ControllerBase
    {
        private readonly GreenPageContext _context;

        public LibroController(GreenPageContext context)
        {
            _context = context;
        }

        /*[HttpGet]
        public async Task<ActionResult<IEnumerable<Libro>>> GetLibros()
        {
            return await _context.Libros.ToListAsync();
        }*/

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Libro>>> GetLibros()
        {
            var libros = await _context.Libros
            .Include(l => l.Editorial)
            .Include(l => l.Genero)
            .Select(l => new
            {
            
                l.IdLibro,
                l.Destitulo,
                l.Estado,
                l.ISBN,
                l.Sinopsys,
                l.IdEditorial,
                l.IdGenero,
                Editorial = l.Editorial.deseditorial,
                Genero = l.Genero.desgenero
            })
            .ToListAsync();

        return Ok(libros);

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Libro>> GetLibro(int id)
        {
            var libro = await _context.Libros.FindAsync(id);

            if (libro == null)
            {
                return NotFound();
            }
            return libro;
        }

        [HttpPost]
        public async Task<ActionResult<Libro>> PostLibro(LibroConSubasta 
        libroConSubasta)
        {

            //Crear y asignar los datos del libro
            var libro = new Libro
            {
                IdEditorial = libroConSubasta.IdEditorial,
                IdGenero = libroConSubasta.IdGenero,
                Destitulo = libroConSubasta.Destitulo,
                Estado = libroConSubasta.Estado,
                ISBN = libroConSubasta.ISBN,
                Sinopsys = libroConSubasta.Sinopsys,
                IdUsuario = libroConSubasta.IdUsuario                
            };

            // Buscar Editorial existente y asignarla al libro
           /* var editorialExistente = await _context.Editoriales.FindAsync(libro.IdEditorial);
            if (editorialExistente != null)
            {
                libro.Editorial = editorialExistente;
            }

            // Buscar Genero existente y asignarlo al libro
            var generoExistente = await _context.Generos.FindAsync(libro.IdGenero);
            if (generoExistente != null)
            {
                libro.Genero = generoExistente;
            }*/

            _context.Libros.Add(libro);
            await _context.SaveChangesAsync();

            // Crear la subasta usando el ID del libro recién creado y los datos de la solicitud
                var nuevaSubasta = new Subasta
                {
                    IdLibro = libro.IdLibro, // Usamos el ID del libro creado
                    FecInicio = libroConSubasta.FecInicio,
                    FecFinal = libroConSubasta.FecFinal,
                    PrecioBase = libroConSubasta.PrecioBase,
                    IdUsuario = libroConSubasta.IdUsuario
                };          

                // Agregar la subasta a la base de datos y guardar
                _context.Subastas.Add(nuevaSubasta);
                await _context.SaveChangesAsync();                  

            return CreatedAtAction(nameof(GetLibro), new { id = libro.IdLibro }, libro);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> PutLibro(int id, LibroUpt libroupt)
        {

            var libro = new Libro
            {
                IdLibro = libroupt.IdLibro,
                IdEditorial = libroupt.IdEditorial,
                IdGenero = libroupt.IdGenero,
                Destitulo = libroupt.Destitulo,
                Estado = libroupt.Estado,
                ISBN = libroupt.ISBN,
                Sinopsys = libroupt.Sinopsys,
                IdUsuario = libroupt.IdUsuario                
            };

            if (id != libro.IdLibro)
            {
                return BadRequest(new { Message = "El ID proporcionado no coincide con el ID del libro." });
            }

            _context.Entry(libro).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { Message = "Actualización satisfactoria." });

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Libros.Any(e => e.IdLibro == id))
                {
                   return NotFound(new { Message = "El libro no fue encontrado." });
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLibro(int id)
        {
            var libro = await _context.Libros.FindAsync(id);
            if (libro == null)
            {
                return NotFound();
            }

            _context.Libros.Remove(libro);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

        public class LibroConSubasta
        {
            // Datos del libro
            public int IdEditorial { get; set; }
            public int IdGenero { get; set; }
            public string Destitulo { get; set; }
            public string Estado { get; set; }
            public string ISBN { get; set; }
            public string Sinopsys { get; set; }
            public int? IdUsuario { get; set; }

            // Datos de la subasta
            public DateTime FecInicio { get; set; }
            public DateTime FecFinal { get; set; }
            public double PrecioBase { get; set; }
        }

        public class LibroUpt
        {
            public int IdLibro {get;set;}
            public int IdEditorial { get; set; }
            public int IdGenero { get; set; }
            public string Destitulo { get; set; }
            public string Estado { get; set; }
            public string ISBN { get; set; }
            public string Sinopsys { get; set; }
            public int? IdUsuario { get; set; }
        }


}
