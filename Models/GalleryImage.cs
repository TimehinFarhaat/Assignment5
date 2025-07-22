using System.ComponentModel.DataAnnotations;

namespace CSharpMvcBasics.Models
{
    public class GalleryImage
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Category { get; set; } // Food, Cloth, Building
        public string ImageUrl { get; set; }
        public DateTime UploadDate { get; set; }
        public bool IsApproved { get; set; }
        [Required]
        public string Hash { get; set; }

    }

}
