using AgenciadeTours.Models;
using Microsoft.EntityFrameworkCore;

namespace AgenciadeTours.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Pais> Paises { get; set; }
        public DbSet<Destino> Destinos { get; set; }
        public DbSet<Tour> Tours { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurando las relaciones con la Tabla Paises

            modelBuilder.Entity<Pais>().HasKey(p => p.PaisID);

            modelBuilder.Entity<Pais>().Property(p => p.Nombre).HasMaxLength(100).IsRequired();

            modelBuilder.Entity<Pais>()
                .HasMany(p => p.Destinos)
                .WithOne(d => d.Pais)
                .HasForeignKey(d => d.PaisID)
                .OnDelete(DeleteBehavior.Cascade);

            // Configurando las relaciones con la tabla Destinos

            modelBuilder.Entity<Destino>().HasKey(d => d.DestinoID);

            modelBuilder.Entity<Destino>().Property(d => d.Nombre).HasMaxLength(150).IsRequired();

            modelBuilder.Entity<Destino>().Property(d => d.Dias_Duracion).IsRequired();

            modelBuilder.Entity<Destino>().Property(d => d.Horas_Duracion).IsRequired();

            modelBuilder.Entity<Destino>()
                .HasOne(d => d.Pais)
                .WithMany(p => p.Destinos)
                .HasForeignKey(d => d.PaisID)
                .OnDelete(DeleteBehavior.Cascade);

            // Configurando las relaciones con la tabla Tours

            modelBuilder.Entity<Tour>().HasKey(t => t.TourID);

            modelBuilder.Entity<Tour>().Property(t => t.Nombre).HasMaxLength(200).IsRequired();

            modelBuilder.Entity<Tour>().Property(t => t.Precio).HasColumnType("decimal(18,2)").IsRequired();

            modelBuilder.Entity<Tour>()
                .HasOne(t => t.Pais)
                .WithMany()
                .HasForeignKey(t => t.PaisID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Tour>()
                .HasOne(t => t.Destino)
                .WithMany()
                .HasForeignKey(t => t.DestinoID)
                .OnDelete(DeleteBehavior.Restrict);

            // Añadiendo Indices Automaticos

            modelBuilder.Entity<Destino>().HasIndex(d => d.PaisID);
            modelBuilder.Entity<Tour>().HasIndex(t => t.PaisID);
            modelBuilder.Entity<Tour>().HasIndex(t => t.DestinoID);
        }
    }
}