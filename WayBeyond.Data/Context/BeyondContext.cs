using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using WayBeyond.Data.Models;

namespace WayBeyond.Data.Context;

public partial class BeyondContext : DbContext
{
    private string _connectString;

    public BeyondContext(string connectionString)
    {
        _connectString = connectionString;
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

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<TexasDebtor> TexasDebtors { get; set; }

    public virtual DbSet<Patient> Patients { get; set; }

    public virtual DbSet<TUResult> TUResults { get; set; }

    public virtual DbSet<ToTransunion> ToTransunions { get; set; }

    public virtual DbSet<ToBadDebt> ToBadDebts { get; set; }
    public virtual DbSet<ToCharity> ToCharities { get; set; }
    public virtual DbSet<ToInventory> ToInventories { get; set; }
    public virtual DbSet<ToCancel> ToCancels { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(_connectString);
      //optionsBuilder.UseSqlite(@"DataSource=\\nmeaf.org\files\Shares\Justin\BACKUP 04222024\Databases\WayBeyond.db;Password=CPAEe9EJ8NMQCKho");
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

        modelBuilder.Entity<TUResult>(entity =>
        {
            entity.HasKey(e => e.MRN);
            entity.ToTable("TUResult");
        });


        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.ClientDebtorNumber);

            entity.ToTable("Account");

            entity.Property(e => e.Inv).HasColumnName("INV");
        });

        modelBuilder.Entity<TexasDebtor>(entity =>
        {
            entity.HasKey(e => e.Mrn);

            entity.ToTable("TexasDebtor");

            entity.Property(e => e.Dob).HasColumnName("DOB");
            entity.Property(e => e.Ssn).HasColumnName("SSN");
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.Mrn);

            entity.ToTable("Patient");

            entity.Property(e => e.Dob).HasColumnName("DOB");
            entity.Property(e => e.Ssn).HasColumnName("SSN");
            entity.Property(e => e.State).HasColumnName("STATE");
        });

        //Table not in db
        modelBuilder.Entity<ToTransunion>(entity =>
        {
            entity.HasNoKey();
        });

        modelBuilder.Entity<ToPIF>(entity =>
        {
            entity.HasNoKey();
        });

        modelBuilder.Entity<ToBadDebt>(entity => { entity.HasNoKey(); });
        modelBuilder.Entity<ToCharity>(entity => { entity.HasNoKey(); });
        modelBuilder.Entity<ToInventory>(entity => { entity.HasNoKey(); });
        modelBuilder.Entity<ToCancel>(entity => { entity.HasNoKey(); });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
