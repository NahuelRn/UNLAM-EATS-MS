// /Services/IProductoService.cs
using ProductosApi.Models;

namespace ProductosApi.Services
{
    public interface IProductoService
    {
        // Contrato para obtener productos con filtros
        Task<IEnumerable<Producto>> GetProductosAsync(string? nombre, decimal? precioMax);

        // Contrato para obtener un producto por ID
        Task<Producto?> GetProductoByIdAsync(int id);

        // Contrato para crear un producto
        Task<Producto> CreateProductoAsync(Producto producto);

        // Contrato para actualizar un producto
        // Devuelve 'true' si fue exitoso, 'false' si no se encontró
        Task<bool> UpdateProductoAsync(int id, Producto producto);

        // Contrato para borrar un producto
        // Devuelve 'true' si fue exitoso, 'false' si no se encontró
        Task<bool> DeleteProductoAsync(int id);

        // Contrato para una regla de negocio específica: chequear nombres duplicados
        // El 'idToIgnore' es para cuando actualizamos (PUT)
        Task<bool> CheckIfNameExistsAsync(string nombre, int? idToIgnore = null);
    }
}