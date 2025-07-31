using ECommerceArtesanos.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ECommerceArtesanos.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Artesano> Artesanos { get; set; }
        public DbSet<Producto> Productos { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Producto>().ToTable("Productos").HasKey(p => p.ProductoId);
            builder.Entity<Artesano>().ToTable("Artesanos").HasKey(a => a.Id);

            // Configura el tipo decimal para Precio
            builder.Entity<Producto>()
                .Property(p => p.Precio)
                .HasColumnType("decimal(18,2)");

            builder.Entity<CarritoItem>()
                .HasOne(ci => ci.Carrito)
                .WithMany(c => c.Items)
                .HasForeignKey(ci => ci.CarritoId);

            base.OnModelCreating(builder);

            builder.Entity<Artesano>(entity =>
            {
                entity.ToTable("Artesanos");
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Nombre).IsRequired();
                entity.Property(a => a.Ubicacion).IsRequired();
            });
        }


        public DbSet<CarritoItem> CarritoItems { get; set; }

        public DbSet<Carrito> Carritos { get; set; }

        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<PedidoItem> PedidoItems { get; set; }


    }
}
