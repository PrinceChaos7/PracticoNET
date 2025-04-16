using ProductCategory.Data;
using Microsoft.EntityFrameworkCore;
using ProductCategory.Models;
using ProductCategory.Services.Interfaces;
using ProductCategory.Services.Implementations;

var builder = WebApplication.CreateBuilder(args);

// Configuraci�n de servicios
builder.Services.AddControllersWithViews();

// Registrar servicios de la capa de negocio
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IProductoService, ProductoService>();

// Configuraci�n de la base de datos en memoria
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("ProductCategoryDB")
          .EnableDetailedErrors()  // Para mejor diagn�stico
          .EnableSensitiveDataLogging());  // Muestra valores de par�metros

var app = builder.Build();

// Configurar pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Inicializaci�n de datos de prueba
await InitializeTestData(app);

app.Run();

async Task InitializeTestData(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var categoriaService = scope.ServiceProvider.GetRequiredService<ICategoriaService>();
    var productoService = scope.ServiceProvider.GetRequiredService<IProductoService>();

    Console.WriteLine("Inicializando datos de prueba...");

    try
    {
        // Crear categor�as si no existen
        if (!await context.Categorias.AnyAsync())
        {
            Console.WriteLine("Creando categor�as de prueba...");

            var categorias = new List<Categoria>
            {
                new() { Nombre = "Electr�nica", Descripcion = "Dispositivos electr�nicos", Activa = true },
                new() { Nombre = "Hogar", Descripcion = "Art�culos para el hogar", Activa = true },
                new() { Nombre = "Congelados", Descripcion = "Art�culos frescos y congelados", Activa = true }
            };

            foreach (var categoria in categorias)
            {
                await categoriaService.CrearCategoriaAsync(categoria);
            }
        }

        // Crear productos si no existen
        if (!await context.Productos.AnyAsync())
        {
            Console.WriteLine("Creando productos de prueba...");

            var catElectronica = await categoriaService.ObtenerCategoriaPorNombreAsync("Electr�nica");
            var catHogar = await categoriaService.ObtenerCategoriaPorNombreAsync("Hogar");
            var catCongelados = await categoriaService.ObtenerCategoriaPorNombreAsync("Congelados");

            var productos = new List<Producto>
            {
                new() {
                    Nombre = "Smartphone",
                    Descripcion = "Tel�fono avanzado",
                    Precio = 599.99m,
                    Stock = 50,
                    CategoriaId = catElectronica.Id
                },
                new() {
                    Nombre = "Sof�",
                    Descripcion = "Sof� de 3 plazas",
                    Precio = 299.99m,
                    Stock = 10,
                    CategoriaId = catHogar.Id
                },
                new() {
                    Nombre = "Helado",
                    Descripcion = "Helado Crufi Triple 1kg",
                    Precio = 550m,
                    Stock = 40,
                    CategoriaId = catCongelados.Id
                }
            };

            foreach (var producto in productos)
            {
                await productoService.CrearProductoAsync(producto);
            }
        }

        // Verificaci�n de datos
        Console.WriteLine($"\nResumen de datos inicializados:");
        Console.WriteLine($"- Categor�as: {await context.Categorias.CountAsync()}");
        Console.WriteLine($"- Productos: {await context.Productos.CountAsync()}");

        // Verificar relaciones
        var productosConCategoria = await context.Productos
            .Include(p => p.Categoria)
            .ToListAsync();

        Console.WriteLine("\nProductos con sus categor�as:");
        foreach (var p in productosConCategoria)
        {
            Console.WriteLine($"{p.Nombre} -> {p.Categoria?.Nombre ?? "Sin categor�a"} (ID: {p.CategoriaId})");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al inicializar datos: {ex.Message}");
        if (ex.InnerException != null)
        {
            Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
        }
    }
}