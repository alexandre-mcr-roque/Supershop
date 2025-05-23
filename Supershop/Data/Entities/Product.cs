﻿using System.ComponentModel.DataAnnotations;

namespace Supershop.Data.Entities
{
    public class Product : IEntity
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "The field {0} can contain {1} characters length.")]
        public string Name { get; set; } = null!;

        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal Price { get; set; }

        [Display(Name = "Image")]
        public Guid ImageId { get; set; }

        [Display(Name = "Last Purchase")]
        public DateTime? LastPurchase { get; set; }

        [Display(Name = "Last Sale")]
        public DateTime? LastSale { get; set; }

        [Display(Name = "Is Available")]
        public bool IsAvailable { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        public double Stock { get; set; }

        public User? User { get; set; }

        public string? ImageFullPath => ImageId == Guid.Empty
            ? $"https://supershop-hfchb0h7cvhtd6fz.uksouth-01.azurewebsites.net/images/noimage.png"
            : $"https://supershopimages.blob.core.windows.net/products/{ImageId}";
        //{
        //    get
        //    {
        //        if (string.IsNullOrEmpty(ImageUrl))
        //        {
        //            return null;
        //        }
        //        return $"https://supershop-hfchb0h7cvhtd6fz.uksouth-01.azurewebsites.net{ImageUrl.Substring(1)}";
        //    }
        //}
    }
}
