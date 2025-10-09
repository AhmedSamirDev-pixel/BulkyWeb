using System.ComponentModel.DataAnnotations;

namespace BulkyWebRazor_Temp.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(30)]
        [Display(Name = "Category Name")]
        public string Name { get; set; }
        [Display(Name = "Display Order")]
        //[Required(ErrorMessage = "Display Order is required")]
        [Range(minimum: 1, maximum: 100, ErrorMessage = "Display Order must be between 1 - 100")]
        public int DisplayOrder { get; set; }
    }
}
