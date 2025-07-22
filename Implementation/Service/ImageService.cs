using CSharpMvcBasics.DTO;
using CSharpMvcBasics.Interface.Repository;
using CSharpMvcBasics.Interface.Services;
using CSharpMvcBasics.Models;
using SixLabors.ImageSharp;
using System.Drawing;


namespace CSharpMvcBasics.Implementation.Service
{
    public class ImageService : IImageService
    {
        private readonly IImageRepository _repo;
        private readonly IGoogleVisionService _visionService;
        private readonly IClarifaiService _clarifaiService;

        public ImageService(IImageRepository repo, IGoogleVisionService visionService, IClarifaiService clarifaiService)
        {
            _repo = repo;
            _visionService = visionService;
            _clarifaiService = clarifaiService;
        }



        public async Task<(bool success, string correctedTitle)> UploadImageAsync(ImageUploadDto dto, string uploadPath)
        {

            string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
            string[] allowedMimeTypes = { "image/jpeg", "image/png", "image/webp" };

            string mimeType = dto.ImageFile.ContentType.ToLower();
            string extension = Path.GetExtension(dto.ImageFile.FileName).ToLower();

            // Check MIME type and extension
            if (!allowedMimeTypes.Contains(mimeType) || !allowedExtensions.Contains(extension))
            {
                return (false, "Only .jpg, .jpeg, .png, or .webp images are allowed.");

            }
            await using var readStream = dto.ImageFile.OpenReadStream();
           
            try
            {
                using var img = await Image.LoadAsync(readStream); // ensure the file is actually an image
                readStream.Position = 0; // Reset for re-use
            }
            catch
            {
                return (false, "The uploaded file is not a valid image.");
            }

            string hash = HashHelper.ComputeSHA256Hash(readStream);
            readStream.Position = 0;
            // Check if hash already exists
            bool exists = await _repo.ImageExist(hash);
            if (exists)
            {
                return (false, "Duplicate image detected.");
            }


            string fileName = Guid.NewGuid() + Path.GetExtension(dto.ImageFile.FileName);
            string filePath = Path.Combine(uploadPath, fileName);
            Directory.CreateDirectory(uploadPath);
            
            // Save file to disk
            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.ImageFile.CopyToAsync(stream);
            }

            // Analyze image with Clarifai
            var (isCategoryMatch, labels) = await _clarifaiService.AnalyzeImageAsync(filePath, dto.Category, dto.Title);

            // If the category doesn't match the image content, reject it
            if (!isCategoryMatch)
            {
                File.Delete(filePath);
                return (false, "Inappropriate Category");
            }

            // Clean and match title
            string cleanedTitle = dto.Title?.Trim().ToLower() ?? "";
            bool titleMatches = labels.Any(label =>
                label.Contains(cleanedTitle) || cleanedTitle.Contains(label)
            );

            string correctedTitle = titleMatches ? dto.Title : labels.FirstOrDefault() ?? dto.Title;


            // Save to database
            var newImage = new GalleryImage
            {
                Title = correctedTitle,
                Category = dto.Category,
                ImageUrl = "/uploads/" + fileName,
                UploadDate = DateTime.Now,
                IsApproved = true,
                Hash = hash
            };

            await _repo.AddAsync(newImage);

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
