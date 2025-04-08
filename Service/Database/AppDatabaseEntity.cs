using Microsoft.EntityFrameworkCore;
using Model.Models;

namespace DynamicPasswordPolicy.Database
{
    public class AppDatabaseEntity : DbContext
    {
        public AppDatabaseEntity(DbContextOptions<AppDatabaseEntity> options) : base(options)
        { }


        public AppDatabaseEntity()
        {
        }

        public DbSet<User> _user { get; set; }
        public DbSet<PasswordPolicy> _passwordPolicies { get; set; }

        public DbSet<PasswordHistory> PasswordHistories { get; set; }

    }
}
