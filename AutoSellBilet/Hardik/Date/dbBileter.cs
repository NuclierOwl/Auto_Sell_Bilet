using System;
using System.Collections.Generic;
using AutoSellBilet.Dao.Model;
using Microsoft.EntityFrameworkCore;

namespace AutoSellBilet.Hardik.Date;

public partial class dbBileter : DbContext
{
    public dbBileter()
    {
    }

    public dbBileter(DbContextOptions<dbBileter> options)
        : base(options)
    {
    }

    public virtual DbSet<Bilet> Bilets { get; set; }

    public virtual DbSet<Bron> Brons { get; set; }

    public virtual DbSet<Kino> Kinos { get; set; }

    public virtual DbSet<Mestum> Mesta { get; set; }

    public virtual DbSet<Sean> Seans { get; set; }

    public virtual DbSet<SeansMestum> SeansMesta { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Zal> Zals { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Server=79.174.88.58;Port=16639;Database=Osipenko;User Name=Osipenko;Password=Osipenko123.;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("en_US.UTF-8")
            .HasPostgresExtension("pg_stat_statements")
            .HasPostgresExtension("uuid-ossp");

        modelBuilder.Entity<Bilet>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("bilets_pkey");

            entity.ToTable("bilets");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval('bilets_id_seq1'::regclass)")
                .HasColumnName("id");
            entity.Property(e => e.FilmBilet).HasColumnName("film_bilet");
            entity.Property(e => e.SeansMestaId).HasColumnName("seans_mesta_id");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'актуален'::character varying")
                .HasColumnType("character varying")
                .HasColumnName("status");
            entity.Property(e => e.ZritelGuid).HasColumnName("zritel_guid");

            entity.HasOne(d => d.FilmBiletNavigation).WithMany(p => p.Bilets)
                .HasPrincipalKey(p => p.Film)
                .HasForeignKey(d => d.FilmBilet)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("bilets_bron_fkey");

            entity.HasOne(d => d.SeansMesta).WithMany(p => p.Bilets)
                .HasForeignKey(d => d.SeansMestaId)
                .HasConstraintName("bilets_seans_mesta_id_fkey");

            entity.HasOne(d => d.Zritel).WithMany(p => p.Bilets)
                .HasForeignKey(d => d.ZritelGuid)
                .HasConstraintName("bilets_zritel_guid_fkey");
        });

        modelBuilder.Entity<Bron>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("bron_pkey");

            entity.ToTable("bron");

            entity.HasIndex(e => e.Film, "bron_film_unique").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Film).HasColumnName("film");
            entity.Property(e => e.Mesto).HasColumnName("mesto");
            entity.Property(e => e.Prise).HasColumnName("prise");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Zritel).HasColumnName("zritel");

            entity.HasOne(d => d.FilmNavigation).WithOne(p => p.Bron)
                .HasForeignKey<Bron>(d => d.Film)
                .HasConstraintName("bron_film_fkey");

            entity.HasOne(d => d.ZritelNavigation).WithMany(p => p.Brons)
                .HasForeignKey(d => d.Zritel)
                .HasConstraintName("bron_zritel_fkey");
        });

        modelBuilder.Entity<Kino>(entity =>
        {
            entity.HasKey(e => e.Name).HasName("kino_pkey");

            entity.ToTable("kino");

            entity.Property(e => e.Name).HasColumnType("character varying");
            entity.Property(e => e.NomerZala).HasColumnName("nomer_zala");
            entity.Property(e => e.Vrema)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("vrema");

            entity.HasOne(d => d.NomerZalaNavigation).WithMany(p => p.Kinos)
                .HasForeignKey(d => d.NomerZala)
                .HasConstraintName("kino_nomer_zala_fkey");
        });

        modelBuilder.Entity<Mestum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("mesta_pkey");

            entity.ToTable("mesta");

            entity.HasIndex(e => new { e.ZalId, e.RowNumber, e.MestoNumber }, "mesta_zal_id_row_number_mesto_number_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval('mesta_id_seq1'::regclass)")
                .HasColumnName("id");
            entity.Property(e => e.MestoNumber).HasColumnName("mesto_number");
            entity.Property(e => e.RowNumber).HasColumnName("row_number");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'свободно'::character varying")
                .HasColumnType("character varying")
                .HasColumnName("status");
            entity.Property(e => e.ZalId).HasColumnName("zal_id");

            entity.HasOne(d => d.Zal).WithMany(p => p.Mesta)
                .HasForeignKey(d => d.ZalId)
                .HasConstraintName("mesta_zal_id_fkey");
        });

        modelBuilder.Entity<Sean>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("seans_pkey");

            entity.ToTable("seans");

            entity.HasIndex(e => new { e.HallId, e.StartTime }, "seanses_hall_id_start_time_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BasePrice)
                .HasPrecision(10, 2)
                .HasColumnName("base_price");
            entity.Property(e => e.HallId).HasColumnName("hall_id");
            entity.Property(e => e.MovieName)
                .HasColumnType("character varying")
                .HasColumnName("movie_name");
            entity.Property(e => e.StartTime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("start_time");

            entity.HasOne(d => d.Hall).WithMany(p => p.Seans)
                .HasForeignKey(d => d.HallId)
                .HasConstraintName("seans_hall_id_fkey");

            entity.HasOne(d => d.MovieNameNavigation).WithMany(p => p.Seans)
                .HasForeignKey(d => d.MovieName)
                .HasConstraintName("seans_movie_name_fkey");
        });

        modelBuilder.Entity<SeansMestum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("seans_mesta_pkey");

            entity.ToTable("seans_mesta");

            entity.HasIndex(e => new { e.SeansId, e.MestoId }, "seans_mesta_unique").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.MestoId).HasColumnName("mesto_id");
            entity.Property(e => e.Price)
                .HasDefaultValueSql("'150'::numeric")
                .HasColumnName("price");
            entity.Property(e => e.SeansId).HasColumnName("seans_id");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'свободно'::character varying")
                .HasColumnType("character varying")
                .HasColumnName("status");

            entity.HasOne(d => d.Mesto).WithMany(p => p.SeansMesta)
                .HasForeignKey(d => d.MestoId)
                .HasConstraintName("seans_mesta_mesto_id_fkey");

            entity.HasOne(d => d.Seans).WithMany(p => p.SeansMesta)
                .HasForeignKey(d => d.SeansId)
                .HasConstraintName("seans_mesta_seans_id_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Guid).HasName("users_pkey");

            entity.ToTable("users");

            entity.Property(e => e.Guid)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("guid");
            entity.Property(e => e.Name).HasColumnType("character varying");
            entity.Property(e => e.Password).HasColumnType("character varying");
        });

        modelBuilder.Entity<Zal>(entity =>
        {
            entity.HasKey(e => e.Nomer).HasName("zals_pkey");

            entity.ToTable("zals");

            entity.Property(e => e.Nomer)
                .ValueGeneratedNever()
                .HasColumnName("nomer");
            entity.Property(e => e.Maxmesto).HasColumnName("maxmesto");
        });
        modelBuilder.HasSequence("bilets_id_seq");
        modelBuilder.HasSequence("mesta_id_seq");
        modelBuilder.HasSequence("seanses_id_seq");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
