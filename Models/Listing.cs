using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;

namespace AuctionHouseApp.Models
{
    public class Listing
    {
        public Listing()
        {
            Bids = new HashSet<Bid>();
            Comments = new HashSet<Comment>();
        }

        public int ListingId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImagePath { get; set; }
        public bool IsSold { get; set; }

        [Required]
        public string? UserId { get; set; }
        public IdentityUser? User { get; set; }

        public ICollection<Bid> Bids { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }
}