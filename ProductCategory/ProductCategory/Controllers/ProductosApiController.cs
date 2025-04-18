using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductCategory.Models;
using ProductCategory.Services.Interfaces;
using System.Threading.Tasks;

namespace ProductCategory.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosApiController : ControllerBase
    {
        private readonly IProductoService _productoService;
        private readonly ICategoriaService _categoriaService;

        public ProductosApiController(
            IProductoService productoService,
            ICategoriaService categoriaService)
        {
            _productoService = productoService;
            _categoriaService = categoriaService;
        }

        // GET: api/ProductosApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProductos(int? pageNumber, int? pageSize)
        {
            int size = pageSize ?? 10;
            var productos = await _productoService.ObtenerTodosProductosQueryable()
                .Include(p => p.Categoria)
                .Skip(((pageNumber ?? 1) - 1) * size)
                .Take(size)
                .ToListAsync();

            return Ok(new
            {
                Data = productos,
                PageNumber = pageNumber ?? 1,
                PageSize = size,
                TotalItems = await _productoService.ObtenerTodosProductosQueryable().CountAsync()
            });
        }

        // GET: api/ProductosApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Producto>> GetProducto(int id)
        {
            var producto = await _productoService.ObtenerProductoPorIdAsync(id);

            if (producto == null)
            {
                return NotFound();
            }

            return producto;
        }

        // GET: api/ProductosApi/search?term=xxx
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Producto>>> SearchProductos(string term, int? pageNumber, int? pageSize)
        {
            if (string.IsNullOrEmpty(term))
            {
                return BadRequest("El término de búsqueda no puede estar vacío");
            }

            int size = pageSize ?? 10;
            var query = _productoService.BuscarProductosQueryable(term);
            var productos = await query
                .Skip(((pageNumber ?? 1) - 1) * size)
                .Take(size)
                .ToListAsync();

            return Ok(new
            {
                Data = productos,
                PageNumber = pageNumber ?? 1,
                PageSize = size,
                TotalItems = await query.CountAsync(),
                SearchTerm = term
            });
        }

        // PUT: api/ProductosApi/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProducto(int id, Producto producto)
        {
            if (id != producto.Id)
            {
                return BadRequest();
            }

            try
            {
                await _productoService.ActualizarProductoAsync(producto);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                if (!await _productoService.ExisteProductoAsync(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // POST: api/ProductosApi
        [HttpPost]
        public async Task<ActionResult<Producto>> PostProducto(Producto producto)
        {
            try
            {
                await _productoService.CrearProductoAsync(producto);
                return CreatedAtAction("GetProducto", new { id = producto.Id }, producto);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/ProductosApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            try
            {
                await _productoService.EliminarProductoAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                if (!await _productoService.ExisteProductoAsync(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }
    }
}