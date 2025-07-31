using ECommerceArtesanos.Data;
using ECommerceArtesanos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerceArtesanos.Controllers
{
    public class ProductosController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ProductosController> _logger;

        public ProductosController(AppDbContext context, ILogger<ProductosController> logger)
        {
            _context = context;
            _logger = logger;
        }


        public async Task<IActionResult> Index()
        {
            try
            {
                var productos = await _context.Productos
                    .Include(p => p.Artesano)
                    .ToListAsync();
                return View(productos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de productos");
                TempData["ErrorMessage"] = "Ocurrió un error al cargar los productos";
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "No se especificó un ID de producto";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var producto = await _context.Productos
                    .Include(p => p.Artesano)
                    .FirstOrDefaultAsync(m => m.ProductoId == id);

                if (producto == null)
                {
                    TempData["ErrorMessage"] = "Producto no encontrado";
                    return RedirectToAction(nameof(Index));
                }

                return View(producto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al mostrar detalles del producto ID: {id}");
                TempData["ErrorMessage"] = "Ocurrió un error al cargar el producto";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Productos/Create
        public IActionResult Create()
        {
            ViewData["ArtesanoId"] = new SelectList(_context.Artesanos, "Id", "Nombre");
            return View();
        }

        // POST: Productos/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Producto producto)
        {
            if (!await _context.Artesanos.AnyAsync(a => a.Id == producto.ArtesanoId))
            {
                ModelState.AddModelError("ArtesanoId", "El artesano seleccionado no existe");
            }

            if (ModelState.IsValid)
            {
                _context.Add(producto);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Producto '{producto.Nombre}' creado exitosamente!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.ArtesanoId = new SelectList(_context.Artesanos, "Id", "Nombre", producto.ArtesanoId);
            return View(producto);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "No se especificó un ID de producto";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var producto = await _context.Productos.FindAsync(id);
                if (producto == null)
                {
                    TempData["ErrorMessage"] = "Producto no encontrado";
                    return RedirectToAction(nameof(Index));
                }

                ViewData["ArtesanoId"] = new SelectList(_context.Artesanos, "Id", "Nombre", producto.ArtesanoId);
                return View(producto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al cargar formulario de edición para producto ID: {id}");
                TempData["ErrorMessage"] = "Ocurrió un error al cargar el formulario";
                return RedirectToAction(nameof(Index));
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductoId,Nombre,Descripcion,Precio,ImagenUrl,ArtesanoId")] Producto producto)
        {
            if (id != producto.ProductoId)
            {
                TempData["ErrorMessage"] = "ID de producto no coincide";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(producto);
                        await _context.SaveChangesAsync();
                        TempData["SuccessMessage"] = $"Producto '{producto.Nombre}' actualizado exitosamente!";
                        return RedirectToAction(nameof(Index));
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        if (!ProductoExists(producto.ProductoId))
                        {
                            TempData["ErrorMessage"] = "El producto ya no existe";
                            return RedirectToAction(nameof(Index));
                        }
                        _logger.LogError(ex, $"Error de concurrencia al editar producto ID: {id}");
                        throw;
                    }
                }

                // Log de errores de validación
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                    .Select(e => e.ErrorMessage)
                                    .ToList();

                _logger.LogWarning("Error de validación al editar producto. Errores: {Errors}", string.Join(", ", errors));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al editar producto ID: {id}");
                ModelState.AddModelError("", "Ocurrió un error al guardar los cambios. Por favor intente nuevamente.");
            }

            ViewData["ArtesanoId"] = new SelectList(_context.Artesanos, "Id", "Nombre", producto.ArtesanoId);
            return View(producto);
        }

        // GET: Productos
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "No se especificó un ID de producto";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var producto = await _context.Productos
                    .Include(p => p.Artesano)
                    .FirstOrDefaultAsync(m => m.ProductoId == id);

                if (producto == null)
                {
                    TempData["ErrorMessage"] = "Producto no encontrado";
                    return RedirectToAction(nameof(Index));
                }

                return View(producto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al cargar confirmación de eliminación para producto ID: {id}");
                TempData["ErrorMessage"] = "Ocurrió un error al cargar la confirmación";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Productos
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var producto = await _context.Productos.FindAsync(id);
                if (producto == null)
                {
                    TempData["ErrorMessage"] = "El producto ya no existe";
                    return RedirectToAction(nameof(Index));
                }

                _context.Productos.Remove(producto);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Producto eliminado exitosamente!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar producto ID: {id}");
                TempData["ErrorMessage"] = "Ocurrió un error al eliminar el producto";
                return RedirectToAction(nameof(Index));
            }
        }

        private bool ProductoExists(int id)
        {
            return _context.Productos.Any(e => e.ProductoId == id);
        }
    }
}