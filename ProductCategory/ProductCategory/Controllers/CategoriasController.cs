using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductCategory.Models;
using ProductCategory.Services.Interfaces;
using System.Threading.Tasks;

namespace ProductCategory.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly ICategoriaService _categoriaService;

        public CategoriasController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        // GET: Categorias
        public async Task<IActionResult> Index(int? pageNumber, int? pageSize)
        {
            int size = pageSize ?? 10; // 10 items por defecto
            var categorias = _categoriaService.ObtenerTodasCategoriasQueryable();
            return View(await PaginatedList<Categoria>.CreateAsync(categorias.AsNoTracking(), pageNumber ?? 1, size));
        }

        // GET: Categorias/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoria = await _categoriaService.ObtenerCategoriaPorIdAsync(id.Value);
            if (categoria == null)
            {
                return NotFound();
            }

            return View(categoria);
        }

        // GET: Categorias/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categorias/ Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nombre,Descripcion,Activa")] Categoria categoria)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _categoriaService.CrearCategoriaAsync(categoria);
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            return View(categoria);
        }

        // GET: Categorias/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoria = await _categoriaService.ObtenerCategoriaPorIdAsync(id.Value);
            if (categoria == null)
            {
                return NotFound();
            }
            return View(categoria);
        }

        // POST: Categorias/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Descripcion,Activa")] Categoria categoria)
        {
            if (id != categoria.Id) return NotFound();

            try
            {
                if (ModelState.IsValid)
                {
                    await _categoriaService.ActualizarCategoriaAsync(categoria);
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            return View(categoria);
        }

        // GET: Categorias/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoria = await _categoriaService.ObtenerCategoriaPorIdAsync(id.Value);
            if (categoria == null)
            {
                return NotFound();
            }

            if (await _categoriaService.TieneProductosAsync(id.Value))
            {
                TempData["Error"] = "No se puede eliminar la categoría porque tiene productos asociados";    //Pero no se muestra
                return RedirectToAction(nameof(Index));
            }

            return View(categoria);
        }

        // POST: Categorias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _categoriaService.EliminarCategoriaAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var categoria = await _categoriaService.ObtenerCategoriaPorIdAsync(id);
                return View("Delete", categoria);
            }
        }

        // GET: Categorias/Search?searchString=xxx
        public async Task<IActionResult> Search(string searchString, int? pageNumber)
        {
            if (string.IsNullOrEmpty(searchString))
            {
                return RedirectToAction(nameof(Index));
            }

            int pageSize = 10; // Cambiado a 10 para consistencia
            var categorias = _categoriaService.BuscarCategoriasQueryable(searchString);

            ViewBag.SearchString = searchString;
            return View("Index", await PaginatedList<Categoria>.CreateAsync(categorias.AsNoTracking(), pageNumber ?? 1, pageSize));
        }
    }
}