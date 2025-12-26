using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using src.Models.CodeFirst.Auditoria;

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
    public virtual DbSet<GrupoPermisos> GruposPermisos { get; set; }
    public virtual DbSet<GrupoPermisoPermiso> GruposPermisosPermisos { get; set; }
    public virtual DbSet<UsuarioGrupoPermisos> UsuariosGruposPermisos { get; set; }
    public virtual DbSet<CompraAuditoria> AuditoriaCompras { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }
    public virtual DbSet<ProductoProveedor> ProductosProveedores { get; set; }
    // public DbSet<Grupo> Grupos { get; set; }
    // public virtual DbSet<ProductoGrupo> ProductosGrupos { get; set; }
    public virtual DbSet<Categoria> Categorias { get; set; }
    public virtual DbSet<CodigoBarra> CodigoBarras { get; set; }
    public virtual DbSet<NovedadesProveedor> NovedadesProveedores { get; set; }
    public virtual DbSet<ProductoCodigoExterno> ProductoCodigosExternos { get; set; }
    public virtual DbSet<ProductoCategoria> ProductoCategorias { get; set; }
    public virtual DbSet<ProductoCodigoBarra> ProductoCodigosBarras { get; set; }
    public virtual DbSet<Familia> Familias { get; set; }

    public virtual DbSet<Compra> Compras { get; set; }
    public virtual DbSet<DetalleCompra> DetallesCompra { get; set; }
    public virtual DbSet<Factura> Facturas { get; set; }
    public virtual DbSet<DetalleFactura> DetallesFactura { get; set; }
    public virtual DbSet<Comprobante> Comprobantes { get; set; }
    public virtual DbSet<NotaCredito> NotasCredito { get; set; }
    public virtual DbSet<NotaDebito> NotasDebito { get; set; }
    public virtual DbSet<MotivoComprobante> MotivosComprobante { get; set; }
    public virtual DbSet<OrdenPago> OrdenesPago { get; set; }
    public virtual DbSet<PagoDetalle> PagosDetalles { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PagoDetalle>()
            .HasKey(pd => new { pd.IdOrdenPago, pd.IdFactura });

        modelBuilder.Entity<Comprobante>().UseTptMappingStrategy();

        modelBuilder.Entity<Comprobante>(entity =>
        {
            // entity.HasKey(e => e.IdComprobante).HasName("comprobante_pkey");
            entity.ToTable("comprobante");
            entity.Property(e => e.IdComprobante)
                  .UseIdentityByDefaultColumn();
        });

        modelBuilder.Entity<NotaCredito>(entity =>
        {
            entity.ToTable("nota_credito");
            // entity.HasKey(e => e.IdComprobante).HasName("pk_nota_credito");
        });

        modelBuilder.Entity<NotaDebito>(entity =>
        {
            entity.ToTable("nota_debito");
            // entity.HasKey(e => e.IdComprobante).HasName("pk_nota_debito");
        });

        modelBuilder.Entity<OrdenPago>(entity =>
        {
            entity.ToTable("orden_pago");
            entity.HasKey(e => e.IdOrdenPago).HasName("orden_pago_pkey");
            entity.Property(e => e.IdOrdenPago).UseIdentityByDefaultColumn();
            entity.HasOne(e => e.Proveedor)
                .WithMany()
                .HasForeignKey(e => e.IdProveedor)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<PagoDetalle>(entity =>
        {
            entity.ToTable("pago_detalle");
            // entity.HasKey(e => e.IdPagoDetalle).HasName("pago_detalle_pkey");
            // entity.Property(e => e.IdPagoDetalle).UseIdentityByDefaultColumn();
            entity.HasOne(d => d.OrdenPago)
                .WithMany(p => p.Detalles)
                .HasForeignKey(d => d.IdOrdenPago)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(d => d.Factura)
                .WithMany()
                .HasForeignKey(d => d.IdFactura)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Compra>(entity =>
        {
            entity.Property(e => e.Estado)
                  .HasConversion<string>();

            // Relación Compra -> Factura
            entity.HasMany(c => c.Facturas)
                  .WithOne(f => f.Compra)
                  .HasForeignKey(f => f.IdCompra)
                  .OnDelete(DeleteBehavior.Restrict); // Evita borrar compras si hay facturas
        });

        modelBuilder.Entity<DetalleCompra>(entity =>
        {
            entity.Property(d => d.PrecioPactado).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Factura>(entity =>
        {
            entity.Property(f => f.TotalFacturado).HasPrecision(18, 2);

            entity.HasOne(f => f.Proveedor)
                  .WithMany()
                  .HasForeignKey(f => f.IdProveedor)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<DetalleFactura>(entity =>
        {
            entity.Property(d => d.PrecioBruto).HasPrecision(18, 2);
            entity.Property(d => d.PrecioNeto).HasPrecision(18, 2);
            entity.Property(d => d.PorcentajeDescuento).HasPrecision(5, 2);

            entity.HasOne(d => d.Factura)
                  .WithMany(f => f.Detalles)
                  .HasForeignKey(d => d.IdFactura)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ProductoProveedor>(entity =>
        {
            entity.Property(u => u.Activo)
                .HasDefaultValue(true);
        });

        modelBuilder.Entity<NovedadesProveedor>()
            .HasOne(n => n.Producto)
            .WithMany(p => p.Novedades)
            .HasForeignKey(n => n.IdProducto)
            .IsRequired(false);

        modelBuilder.Entity<ProductoCodigoExterno>()
            .HasOne(p => p.Producto)
            .WithMany(p => p.CodigosBarrasExternos)
            .HasForeignKey(p => p.IdProducto);

        modelBuilder.Entity<ProductoCodigoExterno>()
            .HasOne(pce => pce.Proveedor)
            .WithMany(p => p.CodigosBarrasExternos)
            .HasForeignKey(pce => pce.IdProveedor);

        modelBuilder.Entity<NovedadesProveedor>()
            .Property(x => x.FechaImportacion)
            .HasColumnType("date");

        modelBuilder.Entity<ProductoCodigoBarra>()
            .HasKey(pg => new { pg.IdProducto, pg.IdCodigoBarra });

        // modelBuilder.Entity<ProductoGrupo>()
        //     .HasKey(pg => new { pg.IdProducto, pg.IdGrupo });

        modelBuilder.Entity<ProductoCategoria>()
            .HasKey(pc => new { pc.IdProducto, pc.IdCategoria });

        modelBuilder.Entity<ProductoCodigoExterno>()
            .HasKey(pce => new { pce.IdProveedor, pce.CodigoBarraProveedor });

        modelBuilder.Entity<NovedadesProveedor>()
            .Property(n => n.Estado)
            .HasConversion<string>();

        modelBuilder.Entity<ProductoProveedor>()
                .HasKey(pp => new { pp.IdProducto, pp.IdProveedor });

        modelBuilder.Entity<ProductoProveedor>()
            .HasOne(pp => pp.Producto)
            .WithMany(p => p.ProductosProveedores)
            .HasForeignKey(pp => pp.IdProducto);

        modelBuilder.Entity<ProductoProveedor>()
            .HasOne(pp => pp.Proveedor)
            .WithMany(p => p.ProductosProveedores)
            .HasForeignKey(pp => pp.IdProveedor);

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

        modelBuilder.Entity<GrupoPermisos>(entity =>
        {
            entity.ToTable("grupo_permisos");
            entity.HasIndex(e => e.Nombre).IsUnique().HasDatabaseName("unq_gpm");
        });

        modelBuilder.Entity<GrupoPermisoPermiso>(entity =>
        {
            entity.ToTable("grupo_permiso_permiso");

            entity.HasKey(gpp => new { gpp.IdGrupoPermiso, gpp.IdPermiso })
                  .HasName("grupo_permiso_permiso_pkey");

            entity.HasOne(gpp => gpp.GrupoPermiso)
                  .WithMany(gp => gp.GruposPermisosPermisos)
                  .HasForeignKey(gpp => gpp.IdGrupoPermiso)
                  .HasConstraintName("gpp_id_grupo_permiso_fkey");

            entity.HasOne(gpp => gpp.Permiso)
                  .WithMany(p => p.GruposPermisosPermisos)
                  .HasForeignKey(gpp => gpp.IdPermiso)
                  .HasConstraintName("gpp_id_permiso_fkey");
        });

        modelBuilder.Entity<UsuarioGrupoPermisos>(entity =>
        {
            entity.ToTable("usuario_grupo_permisos");

            entity.HasKey(ugp => new { ugp.IdUsuario, ugp.IdGrupoPermiso })
                  .HasName("usuario_grupo_permisos_pkey");

            entity.HasOne(ugp => ugp.Usuario)
                  .WithMany(u => u.UsuariosGruposPermisos)
                  .HasForeignKey(ugp => ugp.IdUsuario)
                  .HasConstraintName("ugp_id_usuario_fkey");

            entity.HasOne(ugp => ugp.GrupoPermiso)
                  .WithMany(gp => gp.UsuariosGruposPermisos)
                  .HasForeignKey(ugp => ugp.IdGrupoPermiso)
                  .HasConstraintName("ugp_id_grupo_permiso_fkey");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.Property(u => u.Activo)
                .HasDefaultValue(true);
            entity.HasIndex(e => e.Correo).IsUnique().HasDatabaseName("unq_correo");
            entity.HasIndex(e => e.Identificacion).IsUnique().HasDatabaseName("unq_identificacion");
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

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entradasModificadas = ChangeTracker.Entries()
            .Where(e => (e.Entity is Compra || e.Entity is DetalleCompra) &&
                         e.State == EntityState.Modified)
            .ToList();

        foreach (var entrada in entradasModificadas)
        {
            // Determinamos el ID de la compra padre
            int idCompraPadre = 0;
            string entidadNombre = "";

            if (entrada.Entity is Compra c)
            {
                idCompraPadre = c.IdCompra;
                entidadNombre = "CABECERA_COMPRA";
            }
            else if (entrada.Entity is DetalleCompra d)
            {
                idCompraPadre = d.IdCompra;
                entidadNombre = $"DETALLE_PRODUCTO_{d.IdProducto}";
            }

            string logCambios = GenerarLogDeCambios(entrada);

            if (!string.IsNullOrEmpty(logCambios))
            {
                var auditoria = new CompraAuditoria
                {
                    IdCompra = idCompraPadre,
                    Fecha = DateTime.UtcNow,
                    Accion = $"MODIFICACION_{entidadNombre}",
                    Motivo = (entrada.Entity is Compra comp) ? (comp.Observaciones ?? "sin observaciones") : "Cambio en ítem de la orden",
                    Detalles = logCambios
                };

                this.Set<CompraAuditoria>().Add(auditoria);
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    private string GenerarLogDeCambios(EntityEntry entrada)
    {
        var cambios = new List<string>();

        foreach (var propiedad in entrada.OriginalValues.Properties)
        {
            var original = entrada.OriginalValues[propiedad]?.ToString();
            var actual = entrada.CurrentValues[propiedad]?.ToString();

            if (original != actual)
            {
                cambios.Add($"{propiedad.Name}: '{original}' -> '{actual}'");
            }
        }

        return string.Join(" | ", cambios);
    }
}
