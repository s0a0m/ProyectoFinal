using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace src.Models.CodeFirst;

public partial class AppDbContext : DbContext
{
    // agregar configuracion con metodo OnConfiguring si no se pasa options en el constructor
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.EnableSensitiveDataLogging().LogTo(Console.WriteLine, LogLevel.Information);
        }
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CondicionDePago> Condicion_pagos { get; set; }

    public virtual DbSet<Contado> Contado { get; set; }

    public virtual DbSet<Cuota> Cuota { get; set; }

    public virtual DbSet<Domicilio> Domicilios { get; set; }

    public virtual DbSet<Proveedor> Proveedores { get; set; }

    public virtual DbSet<Provincia> Provincias { get; set; }
    public virtual DbSet<Usuario> Usuarios { get; set; }
    public virtual DbSet<Permiso> Permisos { get; set; }
    public virtual DbSet<UsuarioPermiso> UsuariosPermisos { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Permiso>(entity =>
        {
            entity.ToTable("permiso");
            entity.HasIndex(e => e.Nombre).IsUnique().HasDatabaseName("unq_pm");
        });

        modelBuilder.Entity<UsuarioPermiso>(entity =>
        {
            entity.ToTable("usuario_permiso");

            entity.HasKey(up => new { up.IdPermiso, up.IdUsuario })
                  .HasName("usuario_permiso_pkey");

            entity.HasOne(up => up.Permiso)
                  .WithMany(p => p.UsuariosPermisos)
                  .HasForeignKey(up => up.IdPermiso)
                  .HasConstraintName("usuario_permiso_id_permiso_fkey");

            entity.HasOne(up => up.Usuario)
                  .WithMany(u => u.UsuariosPermisos)
                  .HasForeignKey(up => up.IdUsuario)
                  .HasConstraintName("usuario_permiso_id_usuario_fkey");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.Property(u => u.Activo)
                .HasDefaultValue(true);
        });

        modelBuilder.Entity<CondicionDePago>().UseTptMappingStrategy();
        modelBuilder.Entity<CondicionDePago>(entity =>
        {
            entity.HasKey(e => e.IdCondicionPago).HasName("condicion_pago_pkey");

            entity.ToTable("condicion_pago");

            // entity.Property(e => e.IdCondicionPago).ValueGeneratedNever();
            // entity.Property(e => e.IdCondicionPago).ValueGeneratedOnAdd();
            entity.Property(e => e.IdCondicionPago)
                    .UseIdentityByDefaultColumn();
        });

        modelBuilder.Entity<Contado>(entity =>
        {
            // entity.HasKey(e => e.id_condicion_pago).HasName("contado_pkey");

            entity.ToTable("contado");

            // entity.Property(e => e.IdCondicionPago).ValueGeneratedNever();
        });

        modelBuilder.Entity<Cuota>(entity =>
        {
            // entity.HasKey(e => e.id_condicion_pago).HasName("cuotas_pkey");
            entity.ToTable("cuota");
            // entity.Property(e => e.IdCondicionPago).ValueGeneratedNever();
        });

        modelBuilder.Entity<Domicilio>(entity =>
        {
            entity.HasKey(e => e.IdDomicilio).HasName("domicilio_pkey");

            entity.ToTable("domicilio");

            // entity.Property(e => e.IdDomicilio).ValueGeneratedNever();
            entity.Property(e => e.Calle)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasOne(d => d.IdProvinciaNavigation).WithMany(p => p.Domicilios)
                .HasForeignKey(d => d.IdProvincia)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("direccion_id_provincia_fkey");
        });

        modelBuilder.Entity<Proveedor>(entity =>
        {
            entity.HasKey(e => e.IdProveedor).HasName("proveedor_pkey");

            entity.ToTable("proveedor");

            entity.HasIndex(e => e.Cuit, "unq_cuit").IsUnique();

            entity.HasIndex(e => e.RazonSocial, "unq_razon").IsUnique();

            entity.Property(e => e.IdProveedor).ValueGeneratedOnAdd();
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Correo)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.Cuit)
                .IsRequired()
                .HasMaxLength(11);
            // entity.Property(e => e.domicilio)
            //     .IsRequired()
            //     .HasMaxLength(80);
            entity.Property(e => e.PersonaResponsable)
                .IsRequired()
                .HasMaxLength(80);
            entity.Property(e => e.RazonSocial)
                .IsRequired()
                .HasMaxLength(80);
            entity.Property(e => e.Saldo)
                .IsRequired()
                .HasColumnType("character varying");
            entity.Property(e => e.Telefono)
                .IsRequired()
                .HasMaxLength(12);

            entity.HasOne(d => d.IdCondicionPagoHabitualNavigation).WithMany()
                .HasForeignKey(d => d.IdCondicionPagoHabitual)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("proveedor_id_condicion_pago_habitual_fkey");

            entity.HasOne(d => d.IdDomicilioNavigation).WithMany(p => p.Proveedores)
                .HasForeignKey(d => d.IdDomicilio)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("proveedor_id_domicilio_fkey");
        });

        modelBuilder.Entity<Provincia>(entity =>
        {
            entity.HasKey(e => e.IdProvincia).HasName("provincia_pkey");

            entity.HasIndex(e => e.IdProvincia, "unq_prov").IsUnique();

            entity.Property(e => e.Nombre)
                .IsRequired()
                .HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
