using CSharpMvcBasics.Models;

namespace CSharpMvcBasics.Interface.Repository
{
    public interface IImageRepository
    {
        Task AddAsync(GalleryImage image); 
        Task<IEnumerable<GalleryImage>> GetAllAsync();
        Task<IEnumerable<GalleryImage>> GetFilteredImagesAsync(string category, string sortOrder);
        Task<bool> ImageExist(string computedHash);
    }

}
