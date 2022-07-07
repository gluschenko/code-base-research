using Microsoft.EntityFrameworkCore;
using Wishmaster.DataAccess.Models;

namespace Wishmaster.DataAccess
{
    public class Db : DbContext
    {
        public Db(DbContextOptions<Db> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Space> Spaces { get; set; } = null!;
    }
}
