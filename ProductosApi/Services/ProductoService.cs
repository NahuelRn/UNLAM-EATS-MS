// /Services/ProductoService.cs
using Microsoft.EntityFrameworkCore;
using ProductosApi.Models;

namespace ProductosApi.Services
{
    public class ProductoService : IProductoService
    {
        // ¡El servicio ahora maneja el Context!
        private readonly ProductoContext _context;

        public ProductoService(ProductoContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Producto>> GetProductosAsync(string? nombre, decimal? precioMax)
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

        public async Task<Producto?> GetProductoByIdAsync(int id)
        {
            return await _context.Productos.FindAsync(id);
        }

        public async Task<Producto> CreateProductoAsync(Producto producto)
        {
            // Lógica de negocio movida aquí
            producto.Disponible = (producto.Stock > 0);

            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();
            return producto; // Devuelve el producto creado (con su nuevo Id)
        }

        public async Task<bool> UpdateProductoAsync(int id, Producto producto)
        {
            // Lógica de negocio movida aquí
            producto.Disponible = (producto.Stock > 0);

            _context.Entry(producto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Productos.Any(e => e.Id == id))
                {
                    return false; // No encontrado
                }
                else
                {
                    throw; // Lanzamos la excepción original
                }
            }
            return true; // Éxito
        }

        public async Task<bool> DeleteProductoAsync(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return false; // No encontrado
            }

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();
            return true; // Éxito
        }

        public async Task<bool> CheckIfNameExistsAsync(string nombre, int? idToIgnore = null)
        {
            var query = _context.Productos.AsQueryable();

            // Si estamos actualizando (idToIgnore tiene valor), 
            // excluimos ese ID de la búsqueda de duplicados.
            if (idToIgnore.HasValue)
            {
                query = query.Where(p => p.Id != idToIgnore.Value);
            }

            // Buscamos si algún *otro* producto tiene el mismo nombre
            return await query.AnyAsync(p => p.Nombre.ToLower() == nombre.ToLower());
        }
    }
}