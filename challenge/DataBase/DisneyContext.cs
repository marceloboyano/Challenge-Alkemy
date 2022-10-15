using Microsoft.EntityFrameworkCore;

namespace DataBase
{
    public class DisneyContext : DbContext
    {
        public DisneyContext(DbContextOptions<DisneyContext> options)
             : base(options)
        {

        }
        public DbSet<Gener> Geners { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Character> Characters { get; set; }

        public DbSet<User> Users { get; set; }

    }

}
