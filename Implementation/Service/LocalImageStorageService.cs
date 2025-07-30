using CSharpMvcBasics.Interface.Services;

namespace CSharpMvcBasics.Implementation.Service
{
    public class LocalImageStorageService : IImageStorageService
    {
        private readonly IWebHostEnvironment _env;

        public LocalImageStorageService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName)
        {
            var uploadsPath = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsPath))
                Directory.CreateDirectory(uploadsPath);

            var newFileName = Guid.NewGuid() + Path.GetExtension(fileName);
            var fullPath = Path.Combine(uploadsPath, newFileName);

            using var output = new FileStream(fullPath, FileMode.Create);
            await fileStream.CopyToAsync(output);

            return $"/uploads/{newFileName}";
        }
    }

}
