using Supershop.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace Supershop.Models
{
    public class ProductViewModel : Product
    {
        [Display(Name = "Image")]
        public IFormFile? ImageFile { get; set; }
    }
}
