using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace AuctionHouseApp.Models
{
    public class ListingCreateViewModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Required]
        [Display(Name = "Image File")]
        public IFormFile ImageFile { get; set; } 
    }
}