using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductCategory.Models;
using ProductCategory.Services.Interfaces;
using System.Threading.Tasks;

namespace ProductCategory.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriasApiController : ControllerBase
    {
        private readonly ICategoriaService _categoriaService;

        public CategoriasApiController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        // GET: api/CategoriasApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Categoria>>> GetCategorias(int? pageNumber, int? pageSize)
        {
            int size = pageSize ?? 10;
            var categorias = await _categoriaService.ObtenerTodasCategoriasQueryable()
                .Skip(((pageNumber ?? 1) - 1) * size)
                .Take(size)
                .ToListAsync();

            return Ok(new
            {
                Data = categorias,
                PageNumber = pageNumber ?? 1,
                PageSize = size,
                TotalItems = await _categoriaService.ObtenerTodasCategoriasQueryable().CountAsync()
            });
        }

        // GET: api/CategoriasApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Categoria>> GetCategoria(int id)
        {
            var categoria = await _categoriaService.ObtenerCategoriaPorIdAsync(id);

            if (categoria == null)
            {
                return NotFound();
            }

            return categoria;
        }

        // GET: api/CategoriasApi/search?term=xxx
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Categoria>>> SearchCategorias(string term, int? pageNumber, int? pageSize)
        {
            if (string.IsNullOrEmpty(term))
            {
                return BadRequest("El término de búsqueda no puede estar vacío");
            }

            int size = pageSize ?? 10;
            var query = _categoriaService.BuscarCategoriasQueryable(term);
            var categorias = await query
                .Skip(((pageNumber ?? 1) - 1) * size)
                .Take(size)
                .ToListAsync();

            return Ok(new
            {
                Data = categorias,
                PageNumber = pageNumber ?? 1,
                PageSize = size,
                TotalItems = await query.CountAsync(),
                SearchTerm = term
            });
        }

        // PUT: api/CategoriasApi/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategoria(int id, Categoria categoria)
        {
            if (id != categoria.Id)
            {
                return BadRequest();
            }

            try
            {
                await _categoriaService.ActualizarCategoriaAsync(categoria);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                if (!await _categoriaService.ExisteCategoriaAsync(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // POST: api/CategoriasApi
        [HttpPost]
        public async Task<ActionResult<Categoria>> PostCategoria(Categoria categoria)
        {
            try
            {
                await _categoriaService.CrearCategoriaAsync(categoria);
                return CreatedAtAction("GetCategoria", new { id = categoria.Id }, categoria);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/CategoriasApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoria(int id)
        {
            try
            {
                await _categoriaService.EliminarCategoriaAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                if (!await _categoriaService.ExisteCategoriaAsync(id))
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