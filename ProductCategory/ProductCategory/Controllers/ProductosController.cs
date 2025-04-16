using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProductCategory.Models;
using ProductCategory.Services.Interfaces;
using System.Threading.Tasks;

namespace ProductCategory.Controllers
{
    public class ProductosController : Controller
    {
        private readonly IProductoService _productoService;
        private readonly ICategoriaService _categoriaService;

        public ProductosController(
            IProductoService productoService,
            ICategoriaService categoriaService)
        {
            _productoService = productoService;
            _categoriaService = categoriaService;
        }

        // GET: Productos
        public async Task<IActionResult> Index(int? pageNumber, int? pageSize)
        {
            int size = pageSize ?? 15; // Valor en 15, no es mucho ni poco.
            var productos = _productoService.ObtenerTodosProductosQueryable();
            return View(await PaginatedList<Producto>.CreateAsync(productos.AsNoTracking(), pageNumber ?? 1, size));
        }

        // GET: Productos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _productoService.ObtenerProductoPorIdAsync(id.Value);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // GET: Productos/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                var categorias = await _categoriaService.ObtenerTodasCategoriasAsync();

                if (categorias == null || !categorias.Any())
                {
                    TempData["Error"] = "No hay categorías disponibles. Debe crear al menos una categoría primero.";
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.CategoriaId = new SelectList(categorias, "Id", "Nombre");
                return View();
            }
            catch (Exception ex)
            {
                // Log del error
                Console.WriteLine($"Error al cargar categorías: {ex.Message}");
                TempData["Error"] = "Ocurrió un error al cargar las categorías";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Productos/Create
        // En el método Create (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nombre,Descripcion,Precio,Stock,CategoriaId")] Producto producto)
        {
            try
            {
                // Validación manual adicional
                if (producto.CategoriaId <= 0)
                {
                    ModelState.AddModelError("CategoriaId", "Debe seleccionar una categoría válida");
                }

                if (ModelState.IsValid)
                {
                    await _productoService.CrearProductoAsync(producto);
                    TempData["Success"] = "Producto creado exitosamente";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear producto: {ex}");
                ModelState.AddModelError(string.Empty, $"Error al crear el producto: {ex.Message}");
            }

            await CargarCategoriasViewData(producto.CategoriaId);
            return View(producto);
        }

        // GET: Productos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _productoService.ObtenerProductoPorIdAsync(id.Value);
            if (producto == null)
            {
                return NotFound();
            }

            var categorias = await _categoriaService.ObtenerTodasCategoriasAsync();
            ViewData["CategoriaId"] = new SelectList(categorias, "Id", "Nombre", producto.CategoriaId);
            return View(producto);
        }

        // POST: Productos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Descripcion,Precio,Stock,CategoriaId")] Producto producto)
        {
            if (id != producto.Id) return NotFound();

            try
            {
                if (ModelState.IsValid)
                {
                    await _productoService.ActualizarProductoAsync(producto);
                    TempData["Success"] = "Producto actualizado exitosamente";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al editar producto ID {id}: {ex}");
                ModelState.AddModelError(string.Empty, $"Error al actualizar el producto: {ex.Message}");
            }

            await CargarCategoriasViewData(producto.CategoriaId);
            return View(producto);
        }

        private async Task CargarCategoriasViewData(int? categoriaId = null)
        {
            var categorias = await _categoriaService.ObtenerTodasCategoriasAsync();
            ViewData["CategoriaId"] = new SelectList(categorias, "Id", "Nombre", categoriaId);
        }

        // GET: Productos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _productoService.ObtenerProductoPorIdAsync(id.Value);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // POST: Productos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _productoService.EliminarProductoAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var producto = await _productoService.ObtenerProductoPorIdAsync(id);
                return View("Delete", producto);
            }
        }

        // GET: Productos/Search?searchString=xxx
        public async Task<IActionResult> Search(string searchString, int? pageNumber)
        {
            if (string.IsNullOrEmpty(searchString))
            {
                TempData["Warning"] = "Debe ingresar un término de búsqueda";
                return RedirectToAction(nameof(Index));
            }

            int pageSize = 15;
            var productos = _productoService.BuscarProductosQueryable(searchString);

            ViewBag.SearchString = searchString;
            ViewBag.ResultCount = await productos.CountAsync();

            return View("Index", await PaginatedList<Producto>.CreateAsync(productos, pageNumber ?? 1, pageSize));
        }
    }
}