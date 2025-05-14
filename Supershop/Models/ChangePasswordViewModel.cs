using System.ComponentModel.DataAnnotations;

namespace Supershop.Models
{
    public class ChangePasswordViewModel
    {

        [Required]
        [Display(Name = "Current Password")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required]
        [MinLength(6)]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [Required]
        [Compare(nameof(NewPassword))]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }
    }
}
