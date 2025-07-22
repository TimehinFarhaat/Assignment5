using CSharpMvcBasics.Interface.Repository;
using CSharpMvcBasics.Models;
using Microsoft.EntityFrameworkCore;

namespace CSharpMvcBasics.Implementation.Repository
{
    public class ImageRepository : IImageRepository
    {
        private readonly ApplicationDbContext _context;

        public ImageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(GalleryImage image)
        {
            await _context.GalleryImages.AddAsync(image);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<GalleryImage>> GetAllAsync()
        {
            return await _context.GalleryImages
                                 .Where(i => i.IsApproved)
                                 .ToListAsync();
        }

        public async Task<bool> ImageExist(string computedHash)
        {
            bool exists = await _context.GalleryImages
                 .AnyAsync(i => i.Hash == computedHash);
            return exists;

        }






        public async Task<IEnumerable<GalleryImage>> GetFilteredImagesAsync(string category, string sortOrder)
        {
            var query = _context.GalleryImages.AsQueryable();

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(i => i.Category.ToLower() == category.ToLower());
            }

            query = query.Where(i => i.IsApproved);

            query = sortOrder?.ToLower() switch
            {
                "latest" => query.OrderByDescending(i => i.UploadDate),
                "oldest" => query.OrderBy(i => i.UploadDate),
                _ => query
            };

            return await query.ToListAsync();
        }
    }

}
