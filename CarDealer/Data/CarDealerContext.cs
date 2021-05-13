using CarDealer.Models;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using System;

namespace CarDealer.Data
{
    public class CarDealerContext : DbContext
    {
        public CarDealerContext(DbContextOptions options)
            : base(options)
        {
        }

        public CarDealerContext()
        {
        }

        public DbSet<Car> Cars { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Part> Parts { get; set; }
        public DbSet<PartCar> PartCars { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Server=.;Database=CarDealer;Integrated Security=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PartCar>(e =>
            {
                e.HasKey(k => new { k.CarId, k.PartId });

                e.HasOne(pc => pc.Part)
                .WithMany(p => p.PartCars)
                .HasForeignKey(pc => pc.PartId);

                e.HasOne(pc => pc.Car)
                .WithMany(c => c.PartCars)
                .HasForeignKey(pc => pc.CarId);
            });
        }
    }
}
