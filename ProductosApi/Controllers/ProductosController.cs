// /Controllers/ProductosController.cs
using Microsoft.AspNetCore.Mvc;
using ProductosApi.Models;
using ProductosApi.Services; // <-- Importar el servicio

namespace ProductosApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        // ¡¡El controlador YA NO conoce el DbContext!!
        // Ahora solo conoce el SERVICIO.
        private readonly IProductoService _productoService;

        public ProductosController(IProductoService productoService) // <-- Pide el servicio
        {
            _productoService = productoService;
        }

        // GET: api/Productos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProductos(
            [FromQuery] string? nombre,
            [FromQuery] decimal? precioMax)
        {
            // El controlador solo llama al servicio
            var productos = await _productoService.GetProductosAsync(nombre, precioMax);
            return Ok(productos); // Devuelve una respuesta 200 OK
        }

        // GET: api/Productos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Producto>> GetProducto(int id)
        {
            var producto = await _productoService.GetProductoByIdAsync(id);

            if (producto == null)
            {
                // El controlador decide qué respuesta HTTP dar
                return NotFound(); // 404
            }

            return Ok(producto); // 200
        }

        // PUT: api/Productos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProducto(int id, Producto producto)
        {
            // --- Validación de Request ---
            if (id != producto.Id)
            {
                return BadRequest("El ID de la URL no coincide con el ID del producto.");
            }

            // --- Validación de Modelo (¡AUTOMÁTICA!) ---
            // Gracias a [ApiController] y tus DataAnnotations en Producto.cs
            // (como [Required], [Range], [StringLength]),
            // si el nombre está vacío o el precio es negativo,
            // ASP.NET Core devolverá un 400 BadRequest AUTOMÁTICAMENTE.
            //
            // ¡¡PUEDES BORRAR TODOS TUS 'if' DE VALIDACIÓN MANUAL!!
            // (string.IsNullOrWhiteSpace, producto.Precio < 0, etc.)

            // --- Validación de Lógica de Negocio (Duplicados) ---
            if (await _productoService.CheckIfNameExistsAsync(producto.Nombre, id))
            {
                // Usamos el servicio para la regla de negocio
                return Conflict("Ya existe OTRO producto con ese nombre."); // 409
            }

            // --- Ejecución ---
            var success = await _productoService.UpdateProductoAsync(id, producto);

            if (!success)
            {
                return NotFound(); // 404 (si el servicio no lo encontró)
            }

            return NoContent(); // 204 (Éxito, sin contenido)
        }

        // POST: api/Productos
        [HttpPost]
        public async Task<ActionResult<Producto>> PostProducto(Producto producto)
        {
            // --- Validación de Modelo (AUTOMÁTICA) ---
            // (precio > 0, stock >= 0, nombre requerido, etc.)
            // ¡Ya no necesitas los 'if' manuales!

            // --- Validación de Lógica de Negocio (Duplicados) ---
            if (await _productoService.CheckIfNameExistsAsync(producto.Nombre))
            {
                return Conflict("Ya existe un producto con ese nombre."); // 409
            }

            // --- Ejecución ---
            var productoCreado = await _productoService.CreateProductoAsync(producto);

            // Devolvemos 201 Created
            return CreatedAtAction(
                nameof(GetProducto), // Nombre de la acción para "ver" el recurso
                new { id = productoCreado.Id }, // Parámetro de ruta
                productoCreado); // El objeto creado
        }

        // DELETE: api/Productos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            var success = await _productoService.DeleteProductoAsync(id);

            if (!success)
            {
                return NotFound(); // 404
            }

            return NoContent(); // 204
        }

        // Esta función ya no es necesaria aquí,
        // la lógica similar está en el servicio.
        // private bool ProductoExists(int id) { ... }
    }
}