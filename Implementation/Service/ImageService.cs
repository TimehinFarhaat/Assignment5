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

            using var originalStream = new MemoryStream();
            await dto.ImageFile.CopyToAsync(originalStream);

            // Clone the stream into separate copies
            byte[] imageBytes = originalStream.ToArray();

            try
            {
                using var validateStream = new MemoryStream(imageBytes);
                using var img = await Image.LoadAsync(validateStream); // Validate image
            }
            catch
            {
                return (false, "The uploaded file is not a valid image.");
            }

            // Compute hash
            using var hashStream = new MemoryStream(imageBytes);
            string hash = HashHelper.ComputeSHA256Hash(hashStream);

            if (await _repo.ImageExist(hash))
                return (false, "Duplicate image detected.");

            // Upload to storage
            using var uploadStream = new MemoryStream(imageBytes);
            var imageUrl = await _imageStorageService.UploadFileAsync(uploadStream, dto.ImageFile.FileName);

            // Analyze with Clarifai
            using var analyzeStream = new MemoryStream(imageBytes);
            var (isMatch, labels) = await _clarifaiService.AnalyzeImageAsync(analyzeStream, dto.Category, dto.Title);

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
                UploadDate = DateTime.UtcNow
,
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
