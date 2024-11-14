using GreenPageAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GreenPageAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OfertaController : Controller
    {

        private readonly GreenPageContext _context;

        public OfertaController(GreenPageContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Oferta>>> GetOfertas()
        {
            return await _context.Ofertas.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Oferta>> GetOferta(int id)
        {
            var oferta = await _context.Ofertas.FindAsync(id);

            if (oferta == null)
            {
                return NotFound();
            }

            return oferta;
        }

        [HttpPost]
        public async Task<ActionResult<Oferta>> PostOferta(Oferta oferta)
        {
            if (oferta == null)
            {
                return BadRequest("Oferta no puede ser nula.");
            }     

            // Buscar la oferta con el precio más alto para esta subasta
            var ultimaOferta = await _context.Ofertas
                .Where(o => o.IdSubasta == oferta.IdSubasta) // Filtrar por la subasta correspondiente
                .OrderByDescending(o => o.PrecioOferta) // Ordenar por precio en orden descendente
                .FirstOrDefaultAsync();

            // Validar si el precio de la nueva oferta es mayor que el de la última oferta registrada
            if (ultimaOferta != null && oferta.PrecioOferta <= ultimaOferta.PrecioOferta)
            {
                return BadRequest($"El precio de la oferta debe ser mayor al último ofertado: {ultimaOferta.PrecioOferta}.");
            }                
            _context.Ofertas.Add(oferta);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOferta), new { id = oferta.IdOferta }, oferta);
        }

        [HttpGet("GetGanador/{idSubasta}")]      
        public async Task<ActionResult> GetGanador(int idSubasta)
        {
            //ordenamos la oferta por el precio mas alto
            var ofertaGanadora = await _context.Ofertas
            .Where(o => o.IdSubasta == idSubasta)
            .OrderByDescending(o=>o.PrecioOferta)
            .Select(o=> new
            {
                o.PrecioOferta,
                o.IdUsuario,
                Usuario = new
                {
                    o.usuario.IdUsuario,
                    o.usuario.NomUsuario,
                    o.usuario.ApeUsuario,
                    o.usuario.Login
                }
            })
        .FirstOrDefaultAsync();

        //validar si hay oferta ganadora
        if (ofertaGanadora == null)
        {
            return NotFound("No hay ofertas para la subasta");
        }

        // Retornar la oferta ganadora con los detalles del usuario

        return Ok(new 
        {
            Mensaje = "Ganador de la subasta",
            PrecioGanador = ofertaGanadora.PrecioOferta,
            UsuarioGanador = $"{ofertaGanadora.Usuario.NomUsuario} {ofertaGanadora.Usuario.ApeUsuario[0]}."
        });

        }  

        [HttpGet("GetOfertasPorUsuario/{usuarioId}")]
        public async Task<ActionResult> GetOfertasPorUsuario(int usuarioId)
        {
            //obtener las ofertas de un usuario especifico
            var ofertasusuario = await _context.Ofertas
            .Where(o =>o.IdUsuario == usuarioId)
            .Select(o=> new
            {
                o.IdOferta,
                o.subasta.Libro.Destitulo,
                o.PrecioOferta,
                o.FecOferta
            })
            .ToListAsync();

            if (ofertasusuario == null || !ofertasusuario.Any())
            {
                return NotFound("No hay ofertas para este usuario.");
            }

            return Ok(ofertasusuario);

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutOferta(int id, Oferta oferta)
        {
            if (id != oferta.IdOferta)
            {
                return BadRequest();
            }

            _context.Entry(oferta).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Ofertas.Any(e => e.IdOferta == id))
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
        public async Task<IActionResult> DeleteOferta(int id)
        {
            var oferta = await _context.Ofertas.FindAsync(id);
            if (oferta == null)
            {
                return NotFound();
            }

            _context.Ofertas.Remove(oferta);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
