using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SiteManage.Data;

public partial class DbPagosContext : DbContext
{
    public DbPagosContext()
    {
    }

    public DbPagosContext(DbContextOptions<DbPagosContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TblPago> TblPagos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TblPago>(entity =>
        {
            entity.HasKey(e => e.Id).IsClustered(false);

            entity.ToTable("TBL_PAGO");

            entity.HasIndex(e => new { e.TipoPago, e.Agencia, e.Terminal, e.Documento, e.Referencia2 }, "IDX_REVERSION").HasFillFactor(100);

            entity.HasIndex(e => new { e.FechaIngreso, e.TipoPago }, "IX_TBL_PAGO")
                .IsClustered()
                .HasFillFactor(90);

            entity.HasIndex(e => new { e.TipoPago, e.FechaIngreso }, "SECODARY").HasFillFactor(80);

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("ID");
            entity.Property(e => e.Agencia)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("AGENCIA");
            entity.Property(e => e.Ajenos)
                .HasColumnType("money")
                .HasColumnName("AJENOS");
            entity.Property(e => e.Autorizacion)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("AUTORIZACION");
            entity.Property(e => e.Autorizaciontercero)
                .HasMaxLength(16)
                .IsUnicode(false)
                .HasDefaultValueSql("('0')")
                .HasColumnName("AUTORIZACIONTERCERO");
            entity.Property(e => e.Comision)
                .HasColumnType("money")
                .HasColumnName("COMISION");
            entity.Property(e => e.CuentaDestino)
                .HasMaxLength(25)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("CUENTA_DESTINO");
            entity.Property(e => e.CuentaOrigen)
                .HasMaxLength(25)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("CUENTA_ORIGEN");
            entity.Property(e => e.Documento)
                .HasMaxLength(25)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("DOCUMENTO");
            entity.Property(e => e.Efectivo)
                .HasColumnType("money")
                .HasColumnName("EFECTIVO");
            entity.Property(e => e.Enviado)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ENVIADO");
            entity.Property(e => e.FechaIngreso)
                .HasColumnType("datetime")
                .HasColumnName("FECHA_INGRESO");
            entity.Property(e => e.FechaPago)
                .HasColumnType("datetime")
                .HasColumnName("FECHA_PAGO");
            entity.Property(e => e.Girosext)
                .HasColumnType("money")
                .HasColumnName("GIROSEXT");
            entity.Property(e => e.Msgid)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MSGID");
            entity.Property(e => e.Pagado)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("PAGADO");
            entity.Property(e => e.Propios)
                .HasColumnType("money")
                .HasColumnName("PROPIOS");
            entity.Property(e => e.Referencia2)
                .HasMaxLength(25)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("REFERENCIA2");
            entity.Property(e => e.Referencia3)
                .HasMaxLength(25)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("REFERENCIA3");
            entity.Property(e => e.Referencia4)
                .HasMaxLength(400)
                .IsUnicode(false)
                .HasColumnName("REFERENCIA4");
            entity.Property(e => e.Refval1)
                .HasColumnType("money")
                .HasColumnName("REFVAL1");
            entity.Property(e => e.Refval2)
                .HasColumnType("money")
                .HasColumnName("REFVAL2");
            entity.Property(e => e.Refval3)
                .HasColumnType("money")
                .HasColumnName("REFVAL3");
            entity.Property(e => e.Refval4)
                .HasColumnType("money")
                .HasColumnName("REFVAL4");
            entity.Property(e => e.ScoCajero)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValueSql("('')")
                .HasColumnName("SCO_CAJERO");
            entity.Property(e => e.Terminal)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("TERMINAL");
            entity.Property(e => e.TipoPago).HasColumnName("TIPO_PAGO");
            entity.Property(e => e.Valor)
                .HasColumnType("money")
                .HasColumnName("VALOR");
            entity.Property(e => e.ValorChq)
                .HasColumnType("money")
                .HasColumnName("VALOR_CHQ");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
