using ECommerceArtesanos.Data;
using ECommerceArtesanos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ECommerceArtesanos.Controllers
{
    public class ArtesanosController : Controller
    {
        private readonly AppDbContext _context;

        public ArtesanosController(AppDbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            var artesanos = await _context.Artesanos.ToListAsync();
            return View(artesanos);
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var artesano = await _context.Artesanos
                .FirstOrDefaultAsync(m => m.Id == id);

            if (artesano == null)
                return NotFound();

            return View(artesano);
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Artesano artesano)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(artesano);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException ex)
            {
                // Captura el error real
                var innerMessage = ex.InnerException?.Message;
                ModelState.AddModelError("", $"Error de BD: {innerMessage}");

                // Log para desarrollo
                Console.WriteLine($"ERROR: {innerMessage}");
            }

            return View(artesano);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var artesano = await _context.Artesanos.FindAsync(id);
            if (artesano == null)
                return NotFound();

            return View(artesano);
        }



        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var artesano = await _context.Artesanos
                .FirstOrDefaultAsync(m => m.Id == id);

            if (artesano == null) return NotFound();

            return View(artesano);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var artesano = await _context.Artesanos.FindAsync(id);
            if (artesano == null) return NotFound();

            _context.Artesanos.Remove(artesano);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


    }
}
