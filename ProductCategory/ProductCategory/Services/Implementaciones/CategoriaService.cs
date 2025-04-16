using Microsoft.EntityFrameworkCore;
using ProductCategory.Data;
using ProductCategory.Models;
using ProductCategory.Services.Interfaces;
using System.Linq;

namespace ProductCategory.Services.Implementations
{
    public class CategoriaService : ICategoriaService
    {
        private readonly AppDbContext _context;

        public CategoriaService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Categoria> CrearCategoriaAsync(Categoria categoria)
        {
            if (await NombreCategoriaExisteAsync(categoria.Nombre))
                throw new InvalidOperationException("Ya existe una categoría con este nombre");

            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();
            return categoria;
        }

        public async Task ActualizarCategoriaAsync(Categoria categoria)
        {
            var categoriaExistente = await _context.Categorias.FindAsync(categoria.Id);
            if (categoriaExistente == null)
                throw new InvalidOperationException("Categoría no encontrada");

            if (await NombreCategoriaExisteAsync(categoria.Nombre, categoria.Id))
                throw new InvalidOperationException("Ya existe una categoría con este nombre");

            // Actualizar solo campos permitidos
            categoriaExistente.Nombre = categoria.Nombre;
            categoriaExistente.Descripcion = categoria.Descripcion;
            categoriaExistente.Activa = categoria.Activa;

            await _context.SaveChangesAsync();
        }

        public async Task EliminarCategoriaAsync(int id)
        {
            if (await TieneProductosAsync(id))
                throw new InvalidOperationException("No se puede eliminar la categoría porque tiene productos asociados");

            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria != null)
            {
                _context.Categorias.Remove(categoria);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Categoria>> ObtenerTodasCategoriasAsync()
        {
            return await _context.Categorias.OrderBy(c => c.Nombre).ToListAsync();
        }

        public IQueryable<Categoria> ObtenerTodasCategoriasQueryable()
        {
            return _context.Categorias.OrderBy(c => c.Nombre);
        }

        public IQueryable<Categoria> BuscarCategoriasQueryable(string searchString)
        {
            return _context.Categorias
                .Where(c => c.Nombre.Contains(searchString) || c.Descripcion.Contains(searchString))
                .OrderBy(c => c.Nombre);
        }

        public async Task<Categoria> ObtenerCategoriaPorIdAsync(int id)
        {
            return await _context.Categorias.FindAsync(id);
        }

        public async Task<Categoria> ObtenerCategoriaPorNombreAsync(string nombre)
        {
            return await _context.Categorias.FirstOrDefaultAsync(c => c.Nombre == nombre);
        }

        public async Task<bool> ExisteCategoriaAsync(int id)
        {
            return await _context.Categorias.AnyAsync(e => e.Id == id);
        }

        public async Task<bool> NombreCategoriaExisteAsync(string nombre, int? idExcluir = null)
        {
            return await _context.Categorias
                .AnyAsync(c => c.Nombre == nombre && (!idExcluir.HasValue || c.Id != idExcluir.Value));
        }

        public async Task<bool> TieneProductosAsync(int categoriaId)
        {
            return await _context.Productos.AnyAsync(p => p.CategoriaId == categoriaId);
        }
    }
}