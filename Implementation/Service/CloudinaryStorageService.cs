using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using CSharpMvcBasics.Interface.Services;

namespace CSharpMvcBasics.Implementation.Service
{
    public class CloudinaryStorageService : IImageStorageService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryStorageService(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName)
        {
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(fileName, fileStream),
                Folder = "gallery"
            };

            var result = await _cloudinary.UploadAsync(uploadParams);
            return result.SecureUrl.ToString();
        }
    }

}
