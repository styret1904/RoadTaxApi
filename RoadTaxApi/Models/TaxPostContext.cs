using System;
using Microsoft.EntityFrameworkCore;

namespace RoadTaxApi.Models
{
    public class TaxPostContext : DbContext
    {

        public TaxPostContext(DbContextOptions<TaxPostContext> options)
             : base(options)
        {
        }

        public DbSet<TaxPost> TaxPosts { get; set; }
    }
}
