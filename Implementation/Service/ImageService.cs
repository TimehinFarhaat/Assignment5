using CSharpMvcBasics.DTO;
using CSharpMvcBasics.Interface.Repository;
using CSharpMvcBasics.Interface.Services;
using CSharpMvcBasics.Models;
using Microsoft.AspNetCore.Hosting;
using SixLabors.ImageSharp;
using System.Drawing;


namespace CSharpMvcBasics.Implementation.Service
{
    public class ImageService : IImageService
    {
        private readonly IImageRepository _repo;
        private readonly IClarifaiService _clarifaiService;
        private readonly IImageStorageService _imageStorageService;

        public ImageService(IImageRepository repo, IClarifaiService clarifaiService, IImageStorageService 
          imageStorageService)
        {
            _repo = repo;
            _clarifaiService = clarifaiService;
           _imageStorageService = imageStorageService;
        }



        public async Task<(bool success, string correctedTitle)> UploadImageAsync(ImageUploadDto dto)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var allowedMimeTypes = new[] { "image/jpeg", "image/png", "image/webp" };

            var extension = Path.GetExtension(dto.ImageFile.FileName).ToLowerInvariant();
            var mimeType = dto.ImageFile.ContentType.ToLowerInvariant();

            if (!allowedExtensions.Contains(extension) || !allowedMimeTypes.Contains(mimeType))
                return (false, "Only .jpg, .jpeg, .png, or .webp images are allowed.");

            var readStream = dto.ImageFile.OpenReadStream();

            try
            {
                using var img = await Image.LoadAsync(readStream); // validate
                readStream.Position = 0;
            }
            catch
            {
                return (false, "The uploaded file is not a valid image.");
            }

            string hash = HashHelper.ComputeSHA256Hash(readStream);
            readStream.Position = 0;

            if (await _repo.ImageExist(hash))
                return (false, "Duplicate image detected.");

          
            readStream.Position = 0;
            var imageUrl = await _imageStorageService.UploadFileAsync(readStream, dto.ImageFile.FileName);

           
            readStream.Position = 0;
            var (isMatch, labels) = await _clarifaiService.AnalyzeImageAsync(readStream, dto.Category, dto.Title);

            if (!isMatch)
                return (false, "Category doesn't match image content.");

            string cleanedTitle = dto.Title?.Trim().ToLowerInvariant() ?? "";
            string correctedTitle = labels.FirstOrDefault(l =>
                l.Contains(cleanedTitle) || cleanedTitle.Contains(l)
            ) ?? labels.FirstOrDefault() ?? dto.Title;


            var image = new GalleryImage
            {
                Title = correctedTitle,
                Category = dto.Category,
                ImageUrl = imageUrl,
                UploadDate = DateTime.Now,
                IsApproved = true,
                Hash = hash
            };

            await _repo.AddAsync(image);

            return (true, correctedTitle == dto.Title ? null : correctedTitle);
        }

        public async Task<IEnumerable<ImageDto>> GetImagesAsync(ImageFilterParamsDto filter)
        {
            var images = await _repo.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(filter.Category))
            {
                images = images.Where(i => i.Category.ToLower() == filter.Category.ToLower());
            }

            images = filter.SortOrder switch
            {
                "latest" => images.OrderByDescending(i => i.UploadDate),
                "oldest" => images.OrderBy(i => i.UploadDate),
                _ => images
            };

            // Map to ImageDto
            var imageDtos = images.Select(i => new ImageDto
            {
                Title = i.Title,
                Category = i.Category,
                ImageUrl = i.ImageUrl
            });

            return imageDtos;
        }
    }


}
