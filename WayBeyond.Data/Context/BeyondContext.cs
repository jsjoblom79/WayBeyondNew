using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using WayBeyond.Data.Models;

namespace WayBeyond.Data.Context;

public partial class BeyondContext : DbContext
{
    public BeyondContext()
    {
    }

    public BeyondContext(DbContextOptions<BeyondContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<ClientLoad> ClientLoads { get; set; }

    public virtual DbSet<DropFormat> DropFormats { get; set; }

    public virtual DbSet<DropFormatDetail> DropFormatDetails { get; set; }

    public virtual DbSet<FileFormat> FileFormats { get; set; }

    public virtual DbSet<FileFormatDetail> FileFormatDetails { get; set; }

    public virtual DbSet<FileLocation> FileLocations { get; set; }

    public virtual DbSet<PostalCode> PostalCodes { get; set; }

    public virtual DbSet<ProcessedFileBatch> ProcessedFileBatches { get; set; }

    public virtual DbSet<RemoteConnection> RemoteConnections { get; set; }

    public virtual DbSet<Setting> Settings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(@"DataSource=\\nmeaf.org\files\Shares\Justin\BACKUP 04222024\Databases\WayBeyond.db;Password=CPAEe9EJ8NMQCKho");
       //optionsBuilder.UseSqlite(@"DataSource=D:\Databases\WayBeyond.db;Password=CPAEe9EJ8NMQCKho");

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasIndex(e => e.Id, "IX_Clients_Id").IsUnique();

            entity.HasOne(d => d.DropFormat).WithMany(p => p.Clients).HasForeignKey(d => d.DropFormatId);

            entity.HasOne(d => d.FileFormat).WithMany(p => p.Clients).HasForeignKey(d => d.FileFormatId);
        });

        modelBuilder.Entity<ClientLoad>(entity =>
        {
            entity.HasIndex(e => e.Id, "IX_ClientLoads_Id").IsUnique();

            entity.Property(e => e.CreateDate).HasColumnName("createDate");

            entity.HasOne(d => d.ProcessedFileBatch).WithMany(p => p.ClientLoads).HasForeignKey(d => d.ProcessedFileBatchId);
        });

        modelBuilder.Entity<DropFormat>(entity =>
        {
            entity.HasIndex(e => e.Id, "IX_DropFormats_Id").IsUnique();
        });

        modelBuilder.Entity<DropFormatDetail>(entity =>
        {
            entity.HasIndex(e => e.Id, "IX_DropFormatDetails_Id").IsUnique();

            entity.HasOne(d => d.DropFormat).WithMany(p => p.DropFormatDetails).HasForeignKey(d => d.DropFormatId);
        });

        modelBuilder.Entity<FileFormat>(entity =>
        {
            entity.HasIndex(e => e.Id, "IX_FileFormats_Id").IsUnique();
        });

        modelBuilder.Entity<FileFormatDetail>(entity =>
        {
            entity.HasIndex(e => e.Id, "IX_FileFormatDetails_Id").IsUnique();

            entity.HasOne(d => d.FileFormat).WithMany(p => p.FileFormatDetails).HasForeignKey(d => d.FileFormatId);
        });

        modelBuilder.Entity<FileLocation>(entity =>
        {
            entity.HasIndex(e => e.Id, "IX_FileLocations_Id").IsUnique();

            entity.HasOne(d => d.RemoteConnection).WithMany(p => p.FileLocations).HasForeignKey(d => d.RemoteConnectionId);
        });

        modelBuilder.Entity<PostalCode>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.City).HasColumnName("city");
            entity.Property(e => e.Lat).HasColumnName("lat");
            entity.Property(e => e.Lng).HasColumnName("lng");
            entity.Property(e => e.StateId).HasColumnName("state_id");
            entity.Property(e => e.StateName).HasColumnName("state_name");
        });

        modelBuilder.Entity<ProcessedFileBatch>(entity =>
        {
            entity.ToTable("ProcessedFileBatch");

            entity.HasIndex(e => e.Id, "IX_ProcessedFileBatch_Id").IsUnique();
        });

        modelBuilder.Entity<RemoteConnection>(entity =>
        {
            entity.HasIndex(e => e.Id, "IX_RemoteConnections_Id").IsUnique();
        });

        modelBuilder.Entity<Setting>(entity =>
        {
            entity.HasIndex(e => e.Id, "IX_Settings_Id").IsUnique();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
