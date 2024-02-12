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
        public DbSet<Prices> Prices { get; set; }
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Prices>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<Prices>()
                .Property(p => p.UpdatedDate)
                .ValueGeneratedOnUpdate();

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Integrated Security=True;MultipleActiveResultSets=true;");
            optionsBuilder.LogTo(Console.WriteLine);

        }

    }

    public class Prices : BaseEntity
    {
        public int Id { get; set; }
        public decimal PriceValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}