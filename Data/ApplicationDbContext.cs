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
        });
    }

}
