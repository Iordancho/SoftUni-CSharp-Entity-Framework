using Microsoft.EntityFrameworkCore;
using P01_StudentSystem.P01_StudentSystem.Data.Models;

namespace P01_StudentSystem.P01_StudentSystem.Data;

public class StudentSystemContext: DbContext
{
    public DbSet<Course> Courses { get; set; }
    public DbSet<Resource> Resources { get; set; }
    public DbSet<Student> Students { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //RESOURCE
        modelBuilder.Entity<Resource>()
            .Property(r => r.Name)
            .HasMaxLength(50)
            .IsUnicode();
        modelBuilder.Entity<Resource>()
            .Property(r => r.Url)
            .IsUnicode(false);

        //COURSE
        modelBuilder.Entity<Course>()
            .Property(r => r.Name)
            .HasMaxLength(80)
            .IsUnicode(true);
        modelBuilder.Entity<Course>()
            .Property(r => r.Description)
            .IsUnicode()
            .IsRequired(false);

        //STUDENT
        modelBuilder.Entity<Student>()
            .Property(s => s.Name)
            .HasMaxLength(100)
            .IsUnicode();
        modelBuilder.Entity<Student>()
            .Property(s => s.PhoneNumber)
            .HasMaxLength(10)
            .IsUnicode(false)
            .IsRequired(false);
        modelBuilder.Entity<Student>()
            .Property(s => s.Birthday)
            .IsRequired(false);
    }
}


