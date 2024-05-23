using AuctionHouseApp.Models;

namespace AuctionHouseApp.Interfaces
{
    public interface IBidService
    {
        Task<List<(Listing, decimal)>> GetBidsByUserAsync(string userId);
        Task AddBidAsync(int listingId, string userId, decimal bidAmount);
    }
}
