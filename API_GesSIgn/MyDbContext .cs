using API_GesSIgn.Models;
using Microsoft.EntityFrameworkCore;

public class MonDbContext : DbContext
{
    // a delete lors de l'ajout d'une table 
    public MonDbContext(DbContextOptions<MonDbContext> options) : base(options)
    {
    }

    public DbSet<School> Ecoles { get; set; }
    
    public DbSet<Sectors> Classes { get; set; }

    public DbSet<Error> Errors { get; set; }

    public DbSet<Building> Buildings { get; set; }

    public DbSet<Presence> Presences { get; set; } 
    
    public DbSet<QCM> QCMs { get; set; }

    public DbSet<School> Schools { get; set; }

    public DbSet<Sectors> Sectors { get; set; }

    public DbSet<Student> Students { get; set; }
    
    public DbSet<Subjects> Subjects { get; set; }

    public DbSet<SubjectsHour> SubjectsHour { get; set; }

    public DbSet<User> Users { get; set; }

    public DbSet<Roles> Roles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = Environment.GetEnvironmentVariable("MYAPP_CONNECTION_STRING");
        Console.WriteLine($"ConnectionString: {connectionString}"); // Ajout debug
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("La chaîne de connexion n'est pas définie dans les variables d'environnement.");
        }
        optionsBuilder.UseSqlServer(connectionString);
    }

}
