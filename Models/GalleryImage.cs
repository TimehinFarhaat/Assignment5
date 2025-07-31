using System.ComponentModel.DataAnnotations;

namespace CSharpMvcBasics.Models
{
    public class GalleryImage
    {
        public int Id { get; set; }
        
        [Required]
        public string Title { get; set; }

        [Required]
        public string Category { get; set; }

        [Required]
        public string ImageUrl { get; set; }
        public DateTime UploadDate { get; set; }
        public bool IsApproved { get; set; }
        [Required]
        public string Hash { get; set; }


    }

}
