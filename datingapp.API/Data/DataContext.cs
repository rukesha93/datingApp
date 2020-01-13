using datingapp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace datingapp.API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) :base(options){}
        
        public DbSet<Values> Values { get; set; }
        public DbSet<User> Users { get; set; }
    }
}