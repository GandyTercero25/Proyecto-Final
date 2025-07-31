using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerceArtesanos.Data;
using ECommerceArtesanos.Models;

namespace ECommerceArtesanos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Solo usuarios autenticados pueden usar el carrito
    public class CarritoApiController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CarritoApiController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Obtener el carrito del usuario autenticado.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<Carrito>> GetCarrito()
        {
            var userId = _userManager.GetUserId(User);

            var carrito = await _context.Carritos
                .Include(c => c.Items)
                    .ThenInclude(i => i.Producto)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (carrito == null)
            {
                carrito = new Carrito { UserId = userId };
                _context.Carritos.Add(carrito);
                await _context.SaveChangesAsync();
            }

            return Ok(carrito);
        }

        /// <summary>
        /// Agregar un producto al carrito.
        /// </summary>
        [HttpPost("agregar")]
        public async Task<IActionResult> AgregarProducto([FromBody] CarritoItem dto)
        {
            var userId = _userManager.GetUserId(User);

            var carrito = await _context.Carritos
                .Include(c => c.Items)
                    .ThenInclude(i => i.Producto)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (carrito == null)
            {
                carrito = new Carrito { UserId = userId };
                _context.Carritos.Add(carrito);
                await _context.SaveChangesAsync();
            }

            var item = carrito.Items.FirstOrDefault(i => i.ProductoId == dto.ProductoId);
            if (item == null)
            {
                carrito.Items.Add(new CarritoItem { ProductoId = dto.ProductoId, Cantidad = dto.Cantidad });
            }
            else
            {
                item.Cantidad += dto.Cantidad;
            }

            await _context.SaveChangesAsync();
            return Ok(carrito);
        }

        /// <summary>
        /// Eliminar un producto del carrito.
        /// </summary>
        [HttpDelete("eliminar/{carritoItemId}")]
        public async Task<IActionResult> EliminarItem(int carritoItemId)
        {
            var item = await _context.CarritoItems.FindAsync(carritoItemId);
            if (item == null) return NotFound();

            _context.CarritoItems.Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Vaciar el carrito del usuario.
        /// </summary>
        [HttpDelete("vaciar")]
        public async Task<IActionResult> VaciarCarrito()
        {
            var userId = _userManager.GetUserId(User);

            var carrito = await _context.Carritos
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (carrito != null && carrito.Items.Any())
            {
                _context.CarritoItems.RemoveRange(carrito.Items);
                await _context.SaveChangesAsync();
            }

            return NoContent();
        }
    }


}
