using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductosApi.Models;
using ProductosApi.Services;

namespace ProductosApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly ProductoContext _context;

        public ProductosController(ProductoContext context)
        {
            _context = context;
        }

        // GET:
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProductos(
            [FromQuery] string? nombre,
            [FromQuery] decimal? precioMax)
        {
            var query = _context.Productos.AsQueryable();

            if (!string.IsNullOrEmpty(nombre))
            {
               
                query = query.Where(p => p.Nombre.Contains(nombre));
            }

            if (precioMax.HasValue)
            {
                query = query.Where(p => p.Precio <= precioMax.Value);
            }

            return await query.ToListAsync();
        }

        // GET:
        [HttpGet("{id}")]
        public async Task<ActionResult<Producto>> GetProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
            {
                return NotFound();
            }

            return producto;
        }

        // PUT:
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProducto(int id, Producto producto)
        {
            if (id != producto.Id)
            {
                return BadRequest();
            }

            if (string.IsNullOrWhiteSpace(producto.Nombre))
            {
                return BadRequest("El nombre del producto no puede estar vacío.");
            }
            if (producto.Nombre.Length > 100)
            {
                return BadRequest("El nombre del producto no puede exceder los 100 caracteres.");
            }

            if (producto.Precio < 0)
            {
                return BadRequest("El precio del producto no puede ser negativo.");
            }
            if (producto.Stock < 0)
            {
                return BadRequest("El stock del producto no puede ser negativo.");
            }

            producto.Disponible = (producto.Stock > 0);

            _context.Entry(producto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductoExists(id))
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

        // POST:
        [HttpPost]
        public async Task<ActionResult<Producto>> PostProducto(Producto producto)
        {
            if (producto.Precio < 0)
            {
                return BadRequest("El precio del producto no puede ser negativo.");
            }
            if (producto.Stock < 0)
            {
                return BadRequest("El stock del producto no puede ser negativo.");
            }
            if (string.IsNullOrWhiteSpace(producto.Nombre))
            {
                return BadRequest("El nombre del producto no puede estar vacío.");
            }
            if (producto.Nombre.Length > 100)
            {
                return BadRequest("El nombre del producto no puede exceder los 100 caracteres.");
            }
            bool nombreYaExiste = await _context.Productos
                                        .AnyAsync(p => p.Nombre.ToLower() == producto.Nombre.ToLower());
            if (nombreYaExiste)
            {
                return Conflict("Ya existe un producto con ese nombre.");
            }
            producto.Disponible = (producto.Stock > 0);
            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProducto), new { id = producto.Id }, producto);
        }

        // DELETE:
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductoExists(int id)
        {
            return _context.Productos.Any(e => e.Id == id);
        }
    }
}
