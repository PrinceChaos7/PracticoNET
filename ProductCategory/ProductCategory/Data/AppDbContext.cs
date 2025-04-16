using ProductCategory.Models;
using Microsoft.EntityFrameworkCore;

namespace ProductCategory.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Versión para trabajar en memoria (Ejercicio 1)
        public AppDbContext() { }

        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Producto> Productos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Solo para Ejercicio 1 (queda en memoria)
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseInMemoryDatabase("ProductCategoryDB");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuraciones adicionales
            modelBuilder.Entity<Producto>(entity =>
            {
                entity.HasOne(p => p.Categoria)
                      .WithMany(c => c.Productos)
                      .HasForeignKey(p => p.CategoriaId)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired(); // Hace que CategoriaId sea requerido pero no la navegación
            });

            modelBuilder.Entity<Categoria>(entity =>
            {
                entity.HasIndex(c => c.Nombre).IsUnique();
            });
        }
    }
}
