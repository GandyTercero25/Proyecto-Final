using ECommerceArtesanos.Data;
using ECommerceArtesanos.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ECommerceArtesanos.Controllers
{
    [Authorize]
    public class CarritoController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CarritoController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

  
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var carrito = await _context.Carritos
                .Include(c => c.Items)
                .ThenInclude(i => i.Producto)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            return View(carrito);
        }


        [HttpPost]
        public async Task<IActionResult> Agregar(int productoId, int cantidad = 1)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var carrito = await _context.Carritos
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (carrito == null)
            {
                carrito = new Carrito { UserId = userId, Items = new List<CarritoItem>() };
                _context.Carritos.Add(carrito);
            }

            var itemExistente = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
            if (itemExistente != null)
            {
                itemExistente.Cantidad += cantidad;
            }
            else
            {
                carrito.Items.Add(new CarritoItem { ProductoId = productoId, Cantidad = cantidad });
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Quitar(int itemId)
        {
            var item = await _context.CarritoItems.FindAsync(itemId);
            if (item != null)
            {
                _context.CarritoItems.Remove(item);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

   
        [HttpPost]
        public async Task<IActionResult> Vaciar()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var carrito = await _context.Carritos
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (carrito != null)
            {
                _context.CarritoItems.RemoveRange(carrito.Items); 
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}
