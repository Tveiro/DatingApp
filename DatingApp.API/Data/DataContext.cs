using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) {}
        public DbSet<Value> Values { get; set; } // Values is going to be the table name in the DB.
        public DbSet<User> Users { get; set; }
        public DbSet<Photo> Photos { get; set; }
    }
}