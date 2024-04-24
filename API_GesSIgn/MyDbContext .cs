using API_GesSIgn.Models;
using Microsoft.EntityFrameworkCore;

public class MonDbContext : DbContext
{
    public DbSet<School> Ecoles { get; set; }
    public DbSet<Class> Classes { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = Environment.GetEnvironmentVariable("MYAPP_CONNECTION_STRING");
        optionsBuilder.UseSqlServer(connectionString);
    }

}
