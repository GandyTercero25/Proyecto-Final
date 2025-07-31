using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ECommerceArtesanos.Data;
using ECommerceArtesanos.Models;

namespace ECommerceArtesanos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Solo usuarios autenticados pueden hacer pedidos
    public class PedidosApiController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public PedidosApiController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Confirma un pedido desde el carrito del usuario autenticado.
        /// </summary>
        [HttpPost("confirmar")]
        public async Task<IActionResult> ConfirmarPedido()
        {
            var userId = _userManager.GetUserId(User);

            var carrito = await _context.Carritos
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (carrito == null || carrito.Items.Count == 0)
            {
                return BadRequest(new { mensaje = "El carrito está vacío o no existe." });
            }

            var pedido = new Pedido
            {
                UserId = userId,
                FechaPedido = DateTime.UtcNow,
                Items = carrito.Items.Select(i => new PedidoItem
                {
                    ProductoId = i.ProductoId,
                    Cantidad = i.Cantidad
                }).ToList()
            };

            _context.Pedidos.Add(pedido);
            _context.CarritoItems.RemoveRange(carrito.Items);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Pedido confirmado exitosamente.",
                pedidoId = pedido.PedidoId,
                fecha = pedido.FechaPedido,
                totalItems = pedido.Items.Count
            });
        }
    }
}