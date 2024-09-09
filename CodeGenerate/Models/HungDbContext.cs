using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CodeGenerate.Models;


namespace CodeGenerate.Models
{
    public class HungDbContext: IdentityDbContext<User> // DbContext
    {
        public HungDbContext(DbContextOptions<HungDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Customer> Customers { get; set; }       
    }
}
