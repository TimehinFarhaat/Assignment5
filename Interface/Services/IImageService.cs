using CSharpMvcBasics.DTO;
using CSharpMvcBasics.Models;

namespace CSharpMvcBasics.Interface.Services
{
    public interface IImageService
    {
        Task<(bool success, string correctedTitle)> UploadImageAsync(ImageUploadDto dto, string uploadPath);



        Task<IEnumerable<ImageDto>> GetImagesAsync(ImageFilterParamsDto filter);
    }
}
