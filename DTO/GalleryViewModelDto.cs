using CSharpMvcBasics.DTO;


namespace CSharpMvcBasics.DTO
{
    public class GalleryViewModelDto
    {
        public ImageUploadDto Upload { get; set; } = new();
        public List<ImageDto> Images { get; set; } = new();
    }

}
