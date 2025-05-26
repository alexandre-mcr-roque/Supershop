using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Supershop.Models
{
    public class ChangeUserViewModel
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone Number")]
        [MaxLength(20, ErrorMessage = "The field {0} can contain {1} characters length.")]
        public string? PhoneNumber { get; set; }

        [MaxLength(100, ErrorMessage = "The field {0} can contain {1} characters length.")]
        public string? Address { get; set; }

        [Display(Name = "City")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a city.")]
        public int CityId { get; set; }
        public IEnumerable<SelectListItem> Cities { get; set; }

        [Display(Name = "Country")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a country.")]
        public int CountryId { get; set; }
        public IEnumerable<SelectListItem> Countries { get; set; }
    }
}
