using System.ComponentModel.DataAnnotations;

namespace CSharpMvcBasics.DTO
{
   
        public class ImageUploadDto
        {
            [Required(ErrorMessage = "Title is required")]
            public string Title { get; set; }

            [Required(ErrorMessage = "Category is required")]
            public string Category { get; set; }

            [Required(ErrorMessage = "Please select an image")]
            public IFormFile ImageFile { get; set; }
        }
    




}
