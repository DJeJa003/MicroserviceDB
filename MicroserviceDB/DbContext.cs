using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroserviceDB
{
    using Microsoft.EntityFrameworkCore;

    public class MyDbContext : DbContext
    {
        public DbSet<MyEntity> MyEntities { get; set; }
        public DbSet<Prices> Prices { get; set; }
        public DbSet<LatestPriceData> LatestPriceData { get; set; }
        public DbSet<PriceEntry> PriceEntries { get; set; }
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Prices entity
            modelBuilder.Entity<Prices>()
                .HasKey(p => p.Id); // Set Id as the primary key
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Integrated Security=True;MultipleActiveResultSets=true;");
            optionsBuilder.LogTo(Console.WriteLine);

        }

    }

    public class MyEntity
    {
        public int Id { get; set; }
    }

    public class Prices : BaseEntity
    {
        public int Id { get; set; }
        public decimal PriceValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class LatestPriceData
    {
        public int Id { get; set; }
        public List<PriceEntry> Prices { get; set; }
    }

    public class PriceEntry
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}