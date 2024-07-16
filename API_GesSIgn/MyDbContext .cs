using API_GesSIgn.Models;
using Microsoft.EntityFrameworkCore;
using System;


/* 
    Créé le : 24 April 2024
    Créé par : NicolasDebras
    Modifications :
        8af292a - add school in subject - NicolasDebras
    d3ac484 - fix SubjectHours - NicolasDebras
    c2a6b42 - add but Request PostSubjectsHour - NicolasDebras
    30169da - update model Presence & Subjects - debrasnicolas
    df56d6e - update foreignkey - NicolasDebras
    54d52e1 - change diff - NicolasDebras
    e692f48 - complete push - NicolasDebras
    8e62303 - updat model with correct ef core - NicolasDebras
    a0fe1f7 - fix - debrasnicolas
    272427f - move file - NicolasDebras
*/


public class MonDbContext : DbContext
{
    // a delete lors de l'ajout d'une table 
    public MonDbContext(DbContextOptions<MonDbContext> options) : base(options)
    {
    }

    public DbSet<School> Schools { get; set; }
    public DbSet<Sectors> Sectors { get; set; }
    public DbSet<Error> Errors { get; set; }
    public DbSet<Building> Buildings { get; set; }
    public DbSet<Presence> Presences { get; set; }

    public DbSet<ProofAbsence> ProofAbsences { get; set; }
    public DbSet<QCM> QCMs { get; set; }

    public DbSet<Question> Questions { get; set; }
    public DbSet<OptionQcm> OptionQcm { get; set; }
    public DbSet<QcmResult> QcmResult { get; set; }
    public DbSet<AnswerQCM> AnswerQCM { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Subjects> Subjects { get; set; }
    public DbSet<SubjectsHour> SubjectsHour { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Roles> Roles { get; set; }
    public DbSet<StudentSubject> StudentSubjects { get; set; }

    /*
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = Environment.GetEnvironmentVariable("MYAPP_CONNECTION_STRING");
        Console.WriteLine($"ConnectionString: {connectionString}"); // Ajout debug
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("La chaîne de connexion n'est pas définie dans les variables d'environnement.");
        }
        optionsBuilder.UseSqlServer(connectionString);
    }*/

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasOne(u => u.User_School)
            .WithMany()
            .HasForeignKey(u => u.User_School_Id);

        modelBuilder.Entity<Student>()
            .HasOne(s => s.Student_User)
            .WithMany()
            .HasForeignKey(s => s.Student_User_Id);

        modelBuilder.Entity<Student>()
            .HasOne(s => s.Student_Sectors)
            .WithMany()
            .HasForeignKey(s => s.Student_Sector_Id);

        modelBuilder.Entity<Subjects>()
            .HasOne(s => s.Subjects_User)
            .WithMany()
            .HasForeignKey(s => s.Subjects_User_Id);

        modelBuilder.Entity<SubjectsHour>()
            .HasOne(sh => sh.SubjectsHour_Subjects)
            .WithMany()
            .HasForeignKey(sh => sh.SubjectsHour_Subjects_Id);

        modelBuilder.Entity<SubjectsHour>()
            .HasOne(sh => sh.SubjectsHour_Bulding)
            .WithMany()
            .HasForeignKey(sh => sh.SubjectsHour_Bulding_Id);

        modelBuilder.Entity<Presence>()
            .HasOne(p => p.Presence_Student)
            .WithMany()
            .HasForeignKey(p => p.Presence_Student_Id)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Presence>()
            .HasOne(p => p.Presence_SubjectsHour)
            .WithMany()
            .HasForeignKey(p => p.Presence_SubjectsHour_Id);

        modelBuilder.Entity<Sectors>()
            .HasOne(s => s.Sectors_School)
            .WithMany()
            .HasForeignKey(s => s.Sectors_School_Id);

        modelBuilder.Entity<StudentSubject>()
           .HasKey(ss => new { ss.StudentSubject_StudentId, ss.StudentSubject_SubjectId });

        modelBuilder.Entity<StudentSubject>()
            .HasOne(ss => ss.StudentSubject_Student)
            .WithMany()
            .HasForeignKey(ss => ss.StudentSubject_StudentId);

        modelBuilder.Entity<StudentSubject>()
            .HasOne(ss => ss.StudentSubject_Subject)
            .WithMany()
            .HasForeignKey(ss => ss.StudentSubject_SubjectId);

        modelBuilder.Entity<Subjects>()
            .HasOne(s => s.Subjects_School)
            .WithMany()
            .HasForeignKey(s => s.Subjects_School_Id);

        modelBuilder.Entity<QCM>()
            .HasOne(q => q.QCM_SubjectHour)
            .WithMany()
            .HasForeignKey(q => q.QCM_SubjectHour_id);

        modelBuilder.Entity<Question>()
            .HasOne(q => q.Question_QCM)
            .WithMany()
            .HasForeignKey(q => q.Question_QCM_Id);

        modelBuilder.Entity<OptionQcm>()
            .HasOne(o => o.OptionQcm_Question)
            .WithMany()
            .HasForeignKey(o => o.OptionQcm_Question_Id);

        modelBuilder.Entity<QcmResult>()
            .HasOne(qr => qr.QcmResult_QCM)
            .WithMany()
            .HasForeignKey(qr => qr.QcmResult_QCM_Id);

        modelBuilder.Entity<QcmResult>()
            .HasOne(qr => qr.QcmResult_Student)
            .WithMany()
            .HasForeignKey(qr => qr.QcmResult_Student_Id);

        modelBuilder.Entity<AnswerQCM>()
            .HasOne(qr => qr.AnswerQCM_QCM)
            .WithMany()
            .HasForeignKey(qr => qr.AnswerQCM_QCM_Id);

        modelBuilder.Entity<AnswerQCM>()
            .HasOne(qr => qr.AnswerQCM_Question)
            .WithMany()
            .HasForeignKey(qr => qr.AnswerQCM_Question_Id);

        modelBuilder.Entity<Presence>()
           .HasOne(p => p.Presence_ProofAbsence)
           .WithMany()
           .HasForeignKey(p => p.Presence_ProofAbsence_Id)
           .OnDelete(DeleteBehavior.SetNull);



    }
}
