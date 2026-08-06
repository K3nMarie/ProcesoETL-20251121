// Adaptado a partir del contexto original (AnaliticalDbElectiva1Context) para reflejar el
// esquema real de la base analítica del Project: Project/SistemaAnalisisOpinionesClientes/DBAnalitica.
#nullable enable
using Microsoft.EntityFrameworkCore;
using E_ETL_electiva1.Entities.Models.Dwh.Dims;
using E_ETL_electiva1.Entities.Models.Dwh.Facts;

namespace E_ETL_electiva1.Data.context;

public partial class AnaliticalDbElectiva1Context : DbContext
{
    public AnaliticalDbElectiva1Context(DbContextOptions<AnaliticalDbElectiva1Context> options)
        : base(options)
    {
    }

    public virtual DbSet<CategoriaProducto> CategoriasProducto { get; set; } = null!;

    public virtual DbSet<Producto> Productos { get; set; } = null!;

    public virtual DbSet<Cliente> Clientes { get; set; } = null!;

    public virtual DbSet<Fuente> Fuentes { get; set; } = null!;

    public virtual DbSet<Opinione> Opiniones { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CategoriaProducto>(entity =>
        {
            entity.ToTable("Categorias_Producto");
            entity.HasKey(e => e.IdCategoria);

            entity.Property(e => e.NombreCategoria)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.HasIndex(e => e.NombreCategoria).IsUnique();
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.ToTable("Productos", tb => tb.HasTrigger("TRG_Productos_Delete"));
            entity.HasKey(e => e.IdProducto);

            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .IsUnicode(false);

            entity.HasOne(d => d.IdCategoriaNavigation).WithMany(p => p.Productos)
                .HasForeignKey(d => d.IdCategoria)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Productos_Categorias");

            entity.HasIndex(e => e.IdCategoria, "IX_Productos_Categoria");
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.ToTable("Clientes", tb => tb.HasTrigger("TRG_Clientes_Delete"));
            entity.HasKey(e => e.IdCliente);

            entity.Property(e => e.IdCliente)
                .HasMaxLength(6)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Fuente>(entity =>
        {
            entity.ToTable("Canales_Origen", tb => tb.HasTrigger("TRG_Canales_Delete"));
            entity.HasKey(e => e.IdCanal);

            entity.Property(e => e.NombreCanal)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.HasIndex(e => e.NombreCanal).IsUnique();
        });

        modelBuilder.Entity<Opinione>(entity =>
        {
            entity.ToTable("Opiniones_Clientes", tb => tb.HasTrigger("TR_Opiniones_Insert"));
            entity.HasKey(e => e.IdOpinion);

            entity.Property(e => e.IdCliente)
                .HasMaxLength(6)
                .IsUnicode(false);
            entity.Property(e => e.Clasificacion)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.Opiniones)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_Opinion_Producto");

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.Opiniones)
                .HasForeignKey(d => d.IdCliente)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_Opinion_Cliente");

            entity.HasOne(d => d.IdCanalNavigation).WithMany(p => p.Opiniones)
                .HasForeignKey(d => d.IdCanal)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_Opinion_Canal");

            entity.HasIndex(e => e.IdProducto, "IX_Opinion_Producto");
            entity.HasIndex(e => e.IdCanal, "IX_Opinion_Canal");
            entity.HasIndex(e => e.FechaOpinion, "IX_Opinion_Fecha");
            entity.HasIndex(e => e.IdCliente, "IX_Opinion_Cliente");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
