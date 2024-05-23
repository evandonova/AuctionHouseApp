using System.Collections.Generic;
using AuctionHouseApp.Models;

namespace AuctionHouseApp.Interfaces
{
    public interface IListingService
    {
        Task<IEnumerable<Listing>> GetListingsAsync(string searchString);
        Task<IEnumerable<Listing>> GetListingsByUserAsync(string userId);
        Task AddListingAsync(Listing listing);
        Task<Listing> GetListingByIdAsync(int listingId);
        Task UpdateListingPriceAsync(int listingId, decimal newPrice);
        Task CloseBiddingAsync(int listingId);
    }
}