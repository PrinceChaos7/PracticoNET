using ProductCategory.Models;
using Microsoft.EntityFrameworkCore;

namespace ProductCategory.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Producto> Productos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuración de Producto
            modelBuilder.Entity<Producto>(entity =>
            {
                entity.Property(p => p.FechaCreacion)
                      .HasDefaultValueSql("GETDATE()");

                entity.HasOne(p => p.Categoria)
                      .WithMany(c => c.Productos)
                      .HasForeignKey(p => p.CategoriaId)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired();
            });

            // Configuración de Categoria
            modelBuilder.Entity<Categoria>(entity =>
            {
                entity.Property(c => c.FechaCreacion)
                      .HasDefaultValueSql("GETDATE()");

                entity.Property(c => c.Activa)
                      .HasDefaultValue(true);

                entity.HasIndex(c => c.Nombre)
                      .IsUnique();
            });
        }
        /*
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuraciones adicionales
            modelBuilder.Entity<Producto>(entity =>
            {
                entity.HasOne(p => p.Categoria)
                      .WithMany(c => c.Productos)
                      .HasForeignKey(p => p.CategoriaId)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired();
            });

            modelBuilder.Entity<Categoria>(entity =>
            {
                entity.HasIndex(c => c.Nombre).IsUnique();
            });
        }
        */
    }
}
