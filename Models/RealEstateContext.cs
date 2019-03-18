
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstate.Models
{
    public class RealEstateContext : DbContext
    {
        public RealEstateContext()  {
        }

        public DbSet<Property> Properties { get; set; }
        public DbSet<PropertyDetails> PropertyDetails { get; set; }
        public DbSet<Image> Images { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseMySql("");
        }

    }
}
