using System.ComponentModel.DataAnnotations;

namespace Supershop.Models
{
    public class CityViewModel
    {
        public int CountryId { get; set; }

        [Required]
        [Display(Name = "City")]

        [MaxLength(50, ErrorMessage = "The field {0} can contain {1} characters length.")]
        public string Name { get; set; }
    }
}
