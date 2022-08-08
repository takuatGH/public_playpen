using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PaxQ1.Models;

namespace PaxQ1.Authorization.Persistence
{
    public class ApplicationDatabaseContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDatabaseContext(DbContextOptions<ApplicationDatabaseContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
