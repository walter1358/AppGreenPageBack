using GreenPageAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GreenPageAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubastaController : Controller
    {
        private readonly GreenPageContext _context;

        public SubastaController(GreenPageContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Subasta>>> GetSubastas()
        {
            return await _context.Subastas.ToListAsync();
        }

        [HttpGet("listarSubastasConDetalles")]
        public async Task<ActionResult<IEnumerable<dynamic>>> ListarSubastasConDetalles()
        {
            var subastasConDetalles = await _context.Subastas
                .Include(s => s.Libro) // Incluimos la relación con Libro
                .Select(s => new 
                {
                    TituloLibro = s.Libro.Destitulo, // Obtenemos el título desde Libro
                    Sinopsis = s.Libro.Sinopsys,
                    FechaInicio = s.FecInicio,
                    FechaFin = s.FecFinal,
                    PrecioBase = s.PrecioBase
                })
                .ToListAsync();

            return Ok(subastasConDetalles);
        }        

        [HttpGet("{id}")]
        public async Task<ActionResult<Subasta>> GetSubasta(int id)
        {
            var subasta = await _context.Subastas.FindAsync(id);

            if (subasta == null)
            {
                return NotFound();
            }

            return subasta;
        }

        [HttpPost]
        public async Task<ActionResult<Subasta>> PostSubasta(Subasta subasta)
        {
            _context.Subastas.Add(subasta);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSubasta), new { id = subasta.IdSubasta }, subasta);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutSubasta(int id, Subasta subasta)
        {
            if (id != subasta.IdSubasta)
            {
                return BadRequest();
            }

            _context.Entry(subasta).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Subastas.Any(e => e.IdSubasta == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubasta(int id)
        {
            var subasta = await _context.Subastas.FindAsync(id);
            if (subasta == null)
            {
                return NotFound();
            }

            _context.Subastas.Remove(subasta);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
