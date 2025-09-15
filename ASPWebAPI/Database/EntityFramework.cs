using Microsoft.EntityFrameworkCore;
using ASPWebAPI.Models;

namespace ASPWebAPI.Database
{
    public class AccountsDBContext : DbContext
    {
        public DbSet<LoginRequest> Accounts { get; set; } // Represents the table in the database
        public AccountsDBContext(DbContextOptions<AccountsDBContext> options) : base(options)
        {
        }
    }
}
