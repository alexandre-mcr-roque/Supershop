using System.ComponentModel.DataAnnotations;

namespace Supershop.Data.Entities
{
    public class City : IEntity
    {
        public int Id { get; set; }


        [Required]
        [Display(Name = "City")]
        [MaxLength(50, ErrorMessage = "The field {0} can contain {1} characters length.")]
        public string Name { get; set; }

        // Foreign key references Country.Id
        public int CountryId { get; set; }
    }
}
