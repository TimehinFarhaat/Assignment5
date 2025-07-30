namespace CSharpMvcBasics.Interface.Services
{
    public interface IImageStorageService
    {
        Task<string> UploadFileAsync(Stream fileStream, string fileName);
    }

}
