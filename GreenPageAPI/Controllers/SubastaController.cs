using GreenPageAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

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
                    idlibro = s.IdLibro,
                    idsubasta = s.IdSubasta,
                    TituloLibro = s.Libro.Destitulo, // Obtenemos el título desde Libro
                    estado = s.Libro.Estado,
                    Sinopsis = s.Libro.Sinopsys,
                    FechaInicio = s.FecInicio,
                    FechaFin = s.FecFinal,
                    PrecioBase = s.PrecioBase,
                    isclosed = s.isclosed
                })
                .OrderByDescending(s => s.FechaFin) // Ordenamos por FechaFin en orden descendente
                .ToListAsync();

            return Ok(subastasConDetalles);
        }        

        [HttpGet("{id}")]
        public async Task<ActionResult<Subasta>> GetSubasta(int id)
        {
            var subasta = await _context.Subastas
                                        .Where(s => s.IdSubasta == id) // Ajusta `Id` al nombre real del campo en `Subasta`
                                        .FirstOrDefaultAsync();

            if (subasta == null)
            {
                return NotFound();
            }

            return Ok(subasta);
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

        [HttpPost("iniciar")]
        public async Task<IActionResult> IniciarSubasta([FromBody] int idSubasta)
        {
            var subasta = await _context.Subastas.FindAsync(idSubasta);
            
            if (subasta == null)
            {
                return NotFound("La subasta especificada no existe o esta cerrada.");
            }
            
            if (subasta.isclosed == true)
            {
                return NotFound("La subasta ya se encuentra cerrada.");
            }
            /*if (subasta.StartTime == null)
            {
                return BadRequest("La subasta no tiene una hora de inicio configurada.");
            }*/


                // Verifica si ya tiene una hora de inicio para evitar duplicar
                if (!subasta.StartTime.HasValue)
                {
                    var tiempoDuracion = TimeSpan.FromSeconds(60);
                    subasta.isclosed = false; 
                    subasta.StartTime = DateTime.UtcNow;
                    subasta.EndTime = subasta.StartTime.Value.Add(tiempoDuracion);

                    _context.Subastas.Update(subasta);
                    await _context.SaveChangesAsync();
                }
                return Ok(new { startTime = subasta.StartTime, endTime = subasta.EndTime });
        }


        // GET: api/subasta/tiempo-restante/{id}
        [HttpGet("tiempo-restante/{id}")]
        public async Task<IActionResult> ObtenerTiempoRestante(int id)
        {
            var subasta = await _context.Subastas.FindAsync(id);
            if (subasta == null || subasta.EndTime == null)
            {
                return NotFound();
            }

            //var tiempoRestante = (subasta.EndTime.Value - DateTime.UtcNow).TotalSeconds;
            //tiempoRestante = Math.Max(0, tiempoRestante);
            var tiempoRestante = (int)Math.Max(0, (subasta.EndTime.Value - DateTime.UtcNow).TotalSeconds);


            return Ok(new { tiempoRestante });
        }


        // GET: api/subasta/tiempo-restante/{id}
        [HttpGet("cerrar-subasta/{id}")]
        public async Task<IActionResult> CerrarSubasta(int id)
        {
            var subasta = await _context.Subastas.FindAsync(id);
            if (subasta == null || subasta.StartTime == null)
            {
                return NotFound("No se puede cerrar una subasta que no esta iniciada.");
            }

            subasta.isclosed = true;  // Iniciamos la subasta con el estado abierto
            _context.Subastas.Update(subasta);
            await _context.SaveChangesAsync();

            return Ok(new { 
                subasta,
                mensaje = "Subasta cerrada correctamente"
            });
        }        


    } //Fin de clase SubastaController


   public class Subastastart
{  
    public int IdSubasta { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
}


}
