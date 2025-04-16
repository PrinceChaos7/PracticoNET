using Microsoft.EntityFrameworkCore;
using ProductCategory.Data;
using ProductCategory.Models;
using ProductCategory.Services.Interfaces;

namespace ProductCategory.Services.Implementations
{
    public class ProductoService : IProductoService
    {
        private readonly AppDbContext _context;

        public ProductoService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Producto> CrearProductoAsync(Producto producto)
        {
            // Validar que la categoría exista
            if (!await _context.Categorias.AnyAsync(c => c.Id == producto.CategoriaId))
            {
                throw new InvalidOperationException("La categoría seleccionada no existe");
            }

            if (await NombreProductoExisteEnCategoriaAsync(producto.Nombre, producto.CategoriaId))
            {
                throw new InvalidOperationException("Ya existe un producto con este nombre en la categoría seleccionada");
            }

            producto.FechaCreacion = DateTime.Now;
            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();
            return producto;
        }

        public async Task ActualizarProductoAsync(Producto producto)
        {
            var productoExistente = await _context.Productos.FindAsync(producto.Id);
            if (productoExistente == null)
                throw new InvalidOperationException("Producto no encontrado");
            //Regla de negocio
            if (await NombreProductoExisteEnCategoriaAsync(producto.Nombre, producto.CategoriaId, producto.Id))
                throw new InvalidOperationException("Ya existe un producto con este nombre en la categoría seleccionada");

            // Actualizar solo los campos permitidos
            productoExistente.Nombre = producto.Nombre;
            productoExistente.Descripcion = producto.Descripcion;
            productoExistente.Precio = producto.Precio;
            productoExistente.Stock = producto.Stock;
            productoExistente.CategoriaId = producto.CategoriaId;

            await _context.SaveChangesAsync();
        }

        public async Task EliminarProductoAsync(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto != null)
            {
                _context.Productos.Remove(producto);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Producto>> ObtenerTodosProductosAsync()
        {
            return await _context.Productos.Include(p => p.Categoria).OrderBy(p => p.Nombre).ToListAsync();
        }

        public async Task<Producto> ObtenerProductoPorIdAsync(int id)
        {
            return await _context.Productos.Include(p => p.Categoria).FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<bool> ExisteProductoAsync(int id)
        {
            return await _context.Productos.AnyAsync(e => e.Id == id);
        }

        public async Task<bool> NombreProductoExisteEnCategoriaAsync(string nombre, int categoriaId, int? idExcluir = null)
        {
            return await _context.Productos
                .AnyAsync(p => p.Nombre == nombre &&
                             p.CategoriaId == categoriaId &&
                             (!idExcluir.HasValue || p.Id != idExcluir.Value));
        }

        public IQueryable<Producto> ObtenerTodosProductosQueryable()
        {
            return _context.Productos
                .Include(p => p.Categoria)
                .OrderBy(p => p.Nombre);
        }

        public IQueryable<Producto> BuscarProductosQueryable(string searchString)
        {
            if (string.IsNullOrWhiteSpace(searchString))
            {
                return ObtenerTodosProductosQueryable();
            }

            searchString = searchString.Trim().ToLower();

            return _context.Productos
                .Include(p => p.Categoria)
                .Where(p => p.Nombre.ToLower().Contains(searchString) ||
                           p.Descripcion.ToLower().Contains(searchString) ||
                           p.Categoria.Nombre.ToLower().Contains(searchString))
                .OrderBy(p => p.Nombre);
        }
    }
}