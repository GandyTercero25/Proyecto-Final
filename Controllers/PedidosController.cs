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
    public class PedidosController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public PedidosController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var pedidos = await _context.Pedidos
                .Where(p => p.UserId == userId)
                .Include(p => p.Items)
                .ThenInclude(i => i.Producto)
                .OrderByDescending(p => p.FechaPedido)
                .ToListAsync();

            return View(pedidos);
        }


        [HttpPost]
        public async Task<IActionResult> Crear()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var carrito = await _context.Carritos
                .Include(c => c.Items)
                .ThenInclude(i => i.Producto)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (carrito == null || !carrito.Items.Any())
            {
                TempData["Error"] = "Tu carrito está vacío.";
                return RedirectToAction("Index", "Carrito");
            }

            var pedido = new Pedido
            {
                UserId = userId,
                FechaPedido = DateTime.Now,
                Items = carrito.Items.Select(i => new PedidoItem
                {
                    ProductoId = i.ProductoId,
                    Cantidad = i.Cantidad,

                }).ToList()
            };

            _context.Pedidos.Add(pedido);
            _context.CarritoItems.RemoveRange(carrito.Items); // Vaciar carrito
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Detalle(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var pedido = await _context.Pedidos
                .Include(p => p.Items)
                .ThenInclude(i => i.Producto)
                .FirstOrDefaultAsync(p => p.PedidoId == id && p.UserId == userId);

            if (pedido == null)
            {
                return NotFound();
            }

            return View(pedido);
        }
    }
}
