using System.ComponentModel.DataAnnotations;

namespace ProductCategory.Models
{
    public class Categoria
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(80, ErrorMessage = "El nombre no puede exceder 80 caracteres")]
        public string Nombre { get; set; }

        [StringLength(300, ErrorMessage = "La descripción no puede exceder 300 caracteres")]
        public string Descripcion { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Creación")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        [Display(Name = "Activa")]
        public bool Activa { get; set; } = true;

        public ICollection<Producto>? Productos { get; set; }
        // La relacion con los Productos.
    }
}
