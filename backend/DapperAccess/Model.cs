using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = "Host=localhost;Database=maindb;Username=rodr;Password=TheDBPass";
        optionsBuilder.UseNpgsql(connectionString);
    }
}


public class Blog
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Url { get; set; }
}
