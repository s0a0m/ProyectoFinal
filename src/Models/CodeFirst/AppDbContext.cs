using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace src.Models.CodeFirst;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<condicion_pago> Condicion_pagos { get; set; }

    public virtual DbSet<contado> Contado { get; set; }

    public virtual DbSet<cuota> Cuota { get; set; }

    public virtual DbSet<domicilio> Domicilios { get; set; }

    public virtual DbSet<proveedor> Proveedores { get; set; }

    public virtual DbSet<provincia> Provincias { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<condicion_pago>(entity =>
        {
            entity.HasKey(e => e.id_condicion_pago).HasName("condicion_pago_pkey");

            entity.ToTable("condicion_pago");

            entity.Property(e => e.id_condicion_pago).ValueGeneratedNever();
        });

        modelBuilder.Entity<contado>(entity =>
        {
            entity.HasKey(e => e.id_condicion_pago).HasName("contado_pkey");

            entity.ToTable("contado");

            entity.Property(e => e.id_condicion_pago).ValueGeneratedNever();
        });

        modelBuilder.Entity<cuota>(entity =>
        {
            entity.HasKey(e => e.id_condicion_pago).HasName("cuotas_pkey");

            entity.Property(e => e.id_condicion_pago).ValueGeneratedNever();
        });

        modelBuilder.Entity<domicilio>(entity =>
        {
            entity.HasKey(e => e.id_domicilio).HasName("domicilio_pkey");

            entity.ToTable("domicilio");

            entity.Property(e => e.id_domicilio).ValueGeneratedNever();
            entity.Property(e => e.calle)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasOne(d => d.id_provinciaNavigation).WithMany(p => p.domicilios)
                .HasForeignKey(d => d.id_provincia)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("direccion_id_provincia_fkey");
        });

        modelBuilder.Entity<proveedor>(entity =>
        {
            entity.HasKey(e => e.id_proveedor).HasName("proveedor_pkey");

            entity.ToTable("proveedor");

            entity.HasIndex(e => e.cuit, "unq_cuit").IsUnique();

            entity.HasIndex(e => e.razon_social, "unq_razon").IsUnique();

            entity.Property(e => e.id_proveedor).ValueGeneratedNever();
            entity.Property(e => e.activo).HasDefaultValue(true);
            entity.Property(e => e.correo)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.cuit)
                .IsRequired()
                .HasMaxLength(11);
            // entity.Property(e => e.domicilio)
            //     .IsRequired()
            //     .HasMaxLength(80);
            entity.Property(e => e.persona_responsable)
                .IsRequired()
                .HasMaxLength(80);
            entity.Property(e => e.razon_social)
                .IsRequired()
                .HasMaxLength(80);
            entity.Property(e => e.saldo)
                .IsRequired()
                .HasColumnType("character varying");
            entity.Property(e => e.telefono)
                .IsRequired()
                .HasMaxLength(12);

            entity.HasOne(d => d.id_condicion_pago_habitualNavigation).WithMany(p => p.proveedores)
                .HasForeignKey(d => d.id_condicion_pago_habitual)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("proveedor_id_condicion_pago_habitual_fkey");

            entity.HasOne(d => d.id_domicilioNavigation).WithMany(p => p.proveedores)
                .HasForeignKey(d => d.id_domicilio)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("proveedor_id_domicilio_fkey");
        });

        modelBuilder.Entity<provincia>(entity =>
        {
            entity.HasKey(e => e.id_provincia).HasName("provincia_pkey");

            entity.HasIndex(e => e.id_provincia, "unq_prov").IsUnique();

            entity.Property(e => e.nombre)
                .IsRequired()
                .HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
