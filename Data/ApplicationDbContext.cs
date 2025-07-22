using Microsoft.EntityFrameworkCore;
using CSharpMvcBasics.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<GalleryImage> GalleryImages { get; set; }
}
