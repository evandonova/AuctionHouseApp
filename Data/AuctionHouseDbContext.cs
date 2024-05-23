using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AuctionHouseApp.Models;

namespace AuctionHouseApp.Data
{
    public class AuctionHouseDbContext : IdentityDbContext
    {
        public AuctionHouseDbContext(DbContextOptions<AuctionHouseDbContext> options)
            : base(options)
        {
        }

        public DbSet<Listing> Listings { get; set; }
        public DbSet<Bid> Bids { get; set; }
        public DbSet<Comment> Comments { get; set; }
    }
}