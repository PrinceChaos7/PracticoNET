using ProductCategory.Models;

namespace ProductCategory.Services.Interfaces
{
    public interface ICategoriaService
    {
        Task<Categoria> CrearCategoriaAsync(Categoria categoria);
        Task ActualizarCategoriaAsync(Categoria categoria);
        Task EliminarCategoriaAsync(int id);
        Task<IEnumerable<Categoria>> ObtenerTodasCategoriasAsync();
        Task<Categoria> ObtenerCategoriaPorIdAsync(int id);
        Task<Categoria> ObtenerCategoriaPorNombreAsync(string nombre);
        Task<bool> ExisteCategoriaAsync(int id);
        Task<bool> NombreCategoriaExisteAsync(string nombre, int? idExcluir = null);
        Task<bool> TieneProductosAsync(int categoriaId);
        IQueryable<Categoria> ObtenerTodasCategoriasQueryable();
        IQueryable<Categoria> BuscarCategoriasQueryable(string searchString);
    }
}