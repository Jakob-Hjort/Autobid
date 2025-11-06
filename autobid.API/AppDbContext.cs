using System;
using autobid.Domain.Auctions;
using autobid.Domain.Services;
using autobid.Domain.Users;
using autobid.Domain.Vehicles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace autobid.Domain.Database.EF;

public class AppDbContext : DbContext
{
    public DbSet<Bid> Bids { get; set; }
    public DbSet<Auction> Auctions { get; set; }
    public DbSet<PrivateCustomer> PrivateCustomers { get; set; }
    public DbSet<CorporateCustomer> CorporateUsers { get; set; }
    public DbSet<ProfessionalPersonalCar> CommercialVehicles { get; set; }
    public DbSet<PrivatePersonalCar> PrivateVehicles { get; set; }
    public DbSet<Truck> Trucks { get; set; }
    public DbSet<Bus> Busses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().UseTptMappingStrategy()
            .HasIndex(user => user.Username)
            .IsUnique();

        modelBuilder.Entity<Vehicle>().UseTptMappingStrategy();

        modelBuilder.Entity<PrivateCustomer>()
        .Property(p => p.CPR)
        .IsFixedLength(true)
        .HasMaxLength(10);

        modelBuilder.Entity<CorporateCustomer>()
        .Property(c => c.CVR)
        .IsFixedLength(true)
        .HasMaxLength(8);

        modelBuilder.Entity<User>()
        .Property(u => u.Username)
        .HasMaxLength(32);

        modelBuilder.Entity<PrivateCustomer>()
        .HasIndex(p => p.CPR)
        .IsUnique();

        modelBuilder.Entity<CorporateCustomer>()
        .HasIndex(c => c.CVR)
        .IsUnique();

        modelBuilder.Entity<Auction>()
            .HasOne(a => a.Vehicle)
            .WithMany()
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Auction>()
            .HasOne(a => a.Seller)
            .WithMany()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Auction>()
            .HasKey(a => a.Id);

        modelBuilder.Entity<Bid>()
            .HasKey(b => b.Id);

        modelBuilder.Entity<Bid>()
            .Property(b => b.Amount)
            .IsRequired();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=autobid.db");
    }
    
}
