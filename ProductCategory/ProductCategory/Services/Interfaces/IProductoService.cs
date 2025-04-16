using ProductCategory.Models;

namespace ProductCategory.Services.Interfaces
{
    public interface IProductoService
    {
        Task<Producto> CrearProductoAsync(Producto producto);
        Task ActualizarProductoAsync(Producto producto);
        Task EliminarProductoAsync(int id);
        Task<IEnumerable<Producto>> ObtenerTodosProductosAsync();
        Task<Producto> ObtenerProductoPorIdAsync(int id);
        Task<bool> ExisteProductoAsync(int id);
        Task<bool> NombreProductoExisteEnCategoriaAsync(string nombre, int categoriaId, int? idExcluir = null);
        IQueryable<Producto> ObtenerTodosProductosQueryable();
        IQueryable<Producto> BuscarProductosQueryable(string searchString);
    }
}