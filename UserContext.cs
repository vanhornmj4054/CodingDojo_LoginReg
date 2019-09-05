using Microsoft.EntityFrameworkCore;
 
namespace LoginReg.Models
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions options) : base(options) { }
        public DbSet<User> Users {get;set;}
        public DbSet<LoginUser> LoginUsers {get;set;}
    }
}