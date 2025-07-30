using CSharpMvcBasics.DTO;
using CSharpMvcBasics.Models;

namespace CSharpMvcBasics.Interface.Services
{
    public interface IImageService
    {
       

         Task<(bool success, string correctedTitle)> UploadImageAsync(ImageUploadDto dto);
        Task<IEnumerable<ImageDto>> GetImagesAsync(ImageFilterParamsDto filter);
    }
}
