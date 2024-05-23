using AuctionHouseApp.Data;
using AuctionHouseApp.Interfaces;
using AuctionHouseApp.Models;
using Microsoft.EntityFrameworkCore;

namespace AuctionHouseApp.Services
{
    public class BidService : IBidService
    {
        private readonly AuctionHouseDbContext _context;

        public BidService(AuctionHouseDbContext context)
        {
            _context = context;
        }

        public async Task<List<(Listing, decimal)>> GetBidsByUserAsync(string userId)
        {
            // Fetch the data first
            var bids = await _context.Bids
                                     .Where(b => b.UserId == userId)
                                     .Include(b => b.User)
                                     .Include(b => b.Listing)
                                     .Select(b => new { b.Listing, b.Price })
                                     .ToListAsync();

            // Convert to tuple list after fetching
            return bids.Select(b => (b.Listing, b.Price)).ToList();
        }

        public async Task AddBidAsync(int listingId, string userId, decimal bidAmount)
        {
            var bid = new Bid
            {
                UserId = userId,
                ListingId = listingId,
                Price = bidAmount
            };

            _context.Bids.Add(bid);
            await _context.SaveChangesAsync();
        }
    }
}
