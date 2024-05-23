namespace AuctionHouseApp.Interfaces
{
    public interface ICommentService
    {
        Task AddCommentAsync(int listingId, string userId, string content);
    }
}
