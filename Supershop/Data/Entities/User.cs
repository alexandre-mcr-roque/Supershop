using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Supershop.Data.Entities
{
    public class User : IdentityUser
    {
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [MaxLength(50, ErrorMessage = "The field {0} can contain {1} characters length.")]
        public string Address { get; set; }

        public int? CityId { get; set; }
        public City? City { get; set; }

        [Display(Name = "Full Name")]
        public string FullName => $"{FirstName} {LastName}";
    }
}
