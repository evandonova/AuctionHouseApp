using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace AuctionHouseApp.Models
{
    public class Comment
    {
        public int CommentId { get; set; }
        public string Content { get; set; }

        [Required]
        public string? UserId { get; set; }
        public IdentityUser? User { get; set; }

        public int? ListingId { get; set; }
        public Listing? Listing { get; set; }
    }
}