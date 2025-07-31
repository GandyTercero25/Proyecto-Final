using ECommerceArtesanos.Data;
using ECommerceArtesanos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceArtesanos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArtesanosApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ArtesanosApiController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene la lista de todos los artesanos.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Artesano>>> GetArtesanos()
        {
            return await _context.Artesanos.ToListAsync();
        }

        /// <summary>
        /// Obtiene un artesano por ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Artesano>> GetArtesano(int id)
        {
            var artesano = await _context.Artesanos.FindAsync(id);

            if (artesano == null)
            {
                return NotFound();
            }

            return artesano;
        }

        /// <summary>
        /// Crea un nuevo artesano.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Artesano>> CreateArtesano(Artesano artesano)
        {
            _context.Artesanos.Add(artesano);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetArtesano), new { id = artesano.Id }, artesano);
        }

        /// <summary>
        /// Actualiza un artesano existente.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateArtesano(int id, Artesano artesano)
        {
            if (id != artesano.Id)
            {
                return BadRequest("El ID no coincide");
            }

            _context.Entry(artesano).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArtesanoExists(id))
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

        /// <summary>
        /// Elimina un artesano por ID.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArtesano(int id)
        {
            var artesano = await _context.Artesanos.FindAsync(id);
            if (artesano == null)
            {
                return NotFound();
            }

            _context.Artesanos.Remove(artesano);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ArtesanoExists(int id)
        {
            return _context.Artesanos.Any(e => e.Id == id);
        }
    }
}
