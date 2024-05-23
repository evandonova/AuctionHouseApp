using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace AuctionHouseApp.Models
{
    public class Bid
    {
        public int BidId { get; set; }
        public decimal Price { get; set; }

        [Required]
        public string? UserId { get; set; }
        public IdentityUser? User { get; set; }

        public int? ListingId { get; set; }
        public Listing? Listing { get; set; }
    }
}