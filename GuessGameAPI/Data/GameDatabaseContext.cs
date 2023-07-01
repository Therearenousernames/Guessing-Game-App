using GuessGameAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace GuessGameAPI.Data
{
    public class GameDatabaseContext : DbContext
    {

        public DbSet<Usernames> Usernames { get; set; }

        public DbSet<GameState> GameStates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usernames>().
                Property(a => a.Username).
                UseCollation("SQL_Latin1_General_CP1_CS_AS");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {


            optionsBuilder.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=GameDatabase;Integrated Security=True;");

        }
    }
        
 
}
