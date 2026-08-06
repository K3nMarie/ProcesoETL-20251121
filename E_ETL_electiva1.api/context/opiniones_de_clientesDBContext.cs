#nullable enable
using System;
using System.Collections.Generic;
using E_ETL_electiva1.api.Models;
using Microsoft.EntityFrameworkCore;

namespace E_ETL_electiva1.api.context;

public partial class opiniones_de_clientesDBContext : DbContext
{
    public opiniones_de_clientesDBContext(DbContextOptions<opiniones_de_clientesDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Categorias> Categorias { get; set; }

    public virtual DbSet<Clientes> Clientes { get; set; }

    public virtual DbSet<Fuente_Datos> Fuente_Datos { get; set; }

    public virtual DbSet<Opiniones> Opiniones { get; set; }

    public virtual DbSet<Productos> Productos { get; set; }

    public virtual DbSet<Redes_Sociales> Redes_Sociales { get; set; }

    public virtual DbSet<Tipos_Fuente> Tipos_Fuente { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Categorias>(entity =>
        {
            entity.HasKey(e => e.IdCategoria).HasName("PK__Categori__A3C02A1038ECEA50");

            entity.Property(e => e.NombreCategoria)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Clientes>(entity =>
        {
            entity.HasKey(e => e.IdCliente);

            entity.ToTable(tb => tb.HasTrigger("trg_clientes_delete"));

            entity.Property(e => e.IdCliente)
                .HasMaxLength(6)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(254)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Fuente_Datos>(entity =>
        {
            entity.HasKey(e => e.IdFuente).HasName("PK__Fuente_D__3D674D00F0B04A48");

            entity.Property(e => e.IdFuente)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.IdTipoFuenteNavigation).WithMany(p => p.Fuente_Datos)
                .HasForeignKey(d => d.IdTipoFuente)
                .HasConstraintName("fk_fuente_tipo");
        });

        modelBuilder.Entity<Opiniones>(entity =>
        {
            entity.HasKey(e => e.IdOpinion).HasName("PK__Opinione__2F8F71D7D07B5388");

            entity.HasIndex(e => e.IdCliente, "idx_op_cliente");

            entity.HasIndex(e => e.Fecha, "idx_op_fecha");

            entity.HasIndex(e => e.IdProducto, "idx_op_producto");

            entity.HasIndex(e => e.IdRedSocial, "idx_op_red");

            entity.Property(e => e.Clasificacion)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Fuente)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.IdCliente)
                .HasMaxLength(6)
                .IsUnicode(false);
            entity.Property(e => e.IdProducto).HasMaxLength(6);

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.Opiniones)
                .HasForeignKey(d => d.IdCliente)
                .HasConstraintName("fk_op_cliente");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.Opiniones)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_op_producto");

            entity.HasOne(d => d.IdRedSocialNavigation).WithMany(p => p.Opiniones)
                .HasForeignKey(d => d.IdRedSocial)
                .HasConstraintName("fk_op_red_social");
        });

        modelBuilder.Entity<Productos>(entity =>
        {
            entity.HasKey(e => e.IdProducto);

            entity.HasIndex(e => e.IdCategoria, "idx_prod_cat");

            entity.Property(e => e.IdProducto).HasMaxLength(6);
            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .IsUnicode(false);

            entity.HasOne(d => d.IdCategoriaNavigation).WithMany(p => p.Productos)
                .HasForeignKey(d => d.IdCategoria)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_prod_cat");
        });

        modelBuilder.Entity<Redes_Sociales>(entity =>
        {
            entity.HasKey(e => e.IdRedSocial).HasName("PK__Redes_So__FCCC5114D9600208");

            entity.ToTable(tb => tb.HasTrigger("trg_redes_sociales_delete"));

            entity.HasIndex(e => e.NombreRedSocial, "UQ__Redes_So__37FD7CC8D4F1A467").IsUnique();

            entity.Property(e => e.NombreRedSocial)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Tipos_Fuente>(entity =>
        {
            entity.HasKey(e => e.IdTipoFuente).HasName("PK__Tipos_Fu__D569B34D1860DA7C");

            entity.HasIndex(e => e.NombreTipo, "UQ__Tipos_Fu__7586661CD1D8BC3F").IsUnique();

            entity.Property(e => e.NombreTipo)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}