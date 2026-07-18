using Microsoft.EntityFrameworkCore;
using SuperStockpileMan.Bus.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperStockpileMan.Bus.Contexts
{
    public class SuperStockpileManContext : DbContext
    {
        public DbSet<CategoryBase> CategoryBases { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<SmallestCategory> SmallestCategories { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Log> Logs { get; set; }
        
        public SuperStockpileManContext(DbContextOptions<SuperStockpileManContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<CategoryBase>()
                .HasIndex(e => e.Name)
                .IsUnique();
            modelBuilder.Entity<Category>();
            modelBuilder.Entity<SmallestCategory>();
            modelBuilder
                .Entity<Item>()
                .HasIndex(e => e.Name)
                .IsUnique();
            modelBuilder
                .Entity<Location>()
                .HasIndex(e => e.Name)
                .IsUnique();
            modelBuilder.Entity<Log>();
        }
    }
}
