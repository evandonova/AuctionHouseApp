using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AuctionHouseApp.Interfaces;
using AuctionHouseApp.Models;
using AuctionHouseApp.Data;

namespace AuctionHouseApp.Services
{
    public class ListingService : IListingService
    {
        private readonly AuctionHouseDbContext _context;

        public ListingService(AuctionHouseDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Listing>> GetListingsAsync(string searchString)
        {
            var query = _context.Listings.Include(l => l.User).Where(l => !l.IsSold);

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(l => l.Title.Contains(searchString));
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Listing>> GetListingsByUserAsync(string userId)
        {
            var listings = _context.Listings
                                .Include(l => l.User)
                                .Where(l => l.UserId == userId)
                                .OrderByDescending(l => l.ListingId);

            return await listings.ToListAsync();
        }

        public async Task AddListingAsync(Listing listing)
        {
            _context.Listings.Add(listing);
            await _context.SaveChangesAsync();
        }

        public async Task<Listing> GetListingByIdAsync(int listingId)
        {
            return await _context.Listings
                .Include(l => l.User)
                .Include(l => l.Comments)
                .Include(l => l.Bids)
                    .ThenInclude(b => b.User)
                .FirstOrDefaultAsync(l => l.ListingId == listingId);
        }

        public async Task UpdateListingPriceAsync(int listingId, decimal newPrice)
        {
            var listing = await _context.Listings.FirstOrDefaultAsync(l => l.ListingId == listingId);
            if (listing != null && !listing.IsSold)
            {
                listing.Price = newPrice;
                await _context.SaveChangesAsync();
            }
        }

        public async Task CloseBiddingAsync(int listingId)
        {
            var listing = await _context.Listings.FindAsync(listingId);
            if (listing != null && !listing.IsSold)
            {
                listing.IsSold = true;
                await _context.SaveChangesAsync();
            }
        }
    }
}