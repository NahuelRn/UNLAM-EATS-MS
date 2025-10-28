using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductosApi.Models;

public partial class Producto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
    public string Nombre { get; set; } = null!;

    [StringLength(500, ErrorMessage = "La descripción no puede exceder los 500 caracteres.")]
    public string? Descripcion { get; set; }

    [Required(ErrorMessage = "El precio es obligatorio.")]
    [Range(0.01, 9999999999999999.99, ErrorMessage = "El precio no puede ser negativo.")]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Precio { get; set; }

    [Required(ErrorMessage = "El stock es obligatorio.")]
    [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo.")]
    public int Stock { get; set; }

    // ⚠️ Asume que ya modificaste la tabla Productos, re-escafoldeaste,
    // y esta propiedad existe.
    [Required(ErrorMessage = "El estado de disponibilidad es obligatorio.")] // <-- MOVER AQUÍ
    public bool Disponible { get; set; } // Atributo aplicado a esta propiedad

}