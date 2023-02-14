using Microsoft.EntityFrameworkCore;
using QuizMe.Helpers;

namespace QuizMe.Model;
public class QuizMeContext : DbContext
{
    private readonly DbPath dbPath;

    public QuizMeContext(DbPath path,DbContextOptions<QuizMeContext> options) : base(options) {
        dbPath = path;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseSqlite($"Filename={dbPath.GetPath()}");
    }

    public DbSet<Test> Tests { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<Answer> Answers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Test>().HasMany(x => x.Questions).WithOne(x => x.Test);
        modelBuilder.Entity<Question>().HasMany(x => x.Answers).WithOne(x => x.Question);
    }
}
