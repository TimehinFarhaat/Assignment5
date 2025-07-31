using Microsoft.EntityFrameworkCore;
using CSharpMvcBasics.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<GalleryImage> GalleryImages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<GalleryImage>(entity =>
        {
            entity.Property(e => e.Title).IsRequired();
            entity.Property(e => e.Category).IsRequired();
            entity.Property(e => e.ImageUrl).IsRequired();
            entity.Property(e => e.Hash).IsRequired();

            // ✅ Ensure UploadDate defaults to current timestamp
            entity.Property(e => e.UploadDate)
                  .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
    }

    // ✅ Only include this method if you're NOT using AddDbContext in Program.cs
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
       

        if (!optionsBuilder.IsConfigured)
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var config = new ConfigurationBuilder()
         .SetBasePath(Directory.GetCurrentDirectory())
         .AddJsonFile($"appsettings.{env}.json", optional: true)
         .AddEnvironmentVariables()
         .Build();

            if (env == "Development")
            {
                optionsBuilder.UseSqlServer(config.GetConnectionString("DefaultConnection"));
            }
            else
            {
                optionsBuilder.UseNpgsql(config.GetConnectionString("PostgresConnection"));
            }
        }
    }
}
