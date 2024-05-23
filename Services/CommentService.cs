using AuctionHouseApp.Data;
using AuctionHouseApp.Interfaces;
using AuctionHouseApp.Models;

namespace AuctionHouseApp.Services
{
    public class CommentService : ICommentService
    {
        private readonly AuctionHouseDbContext _context;

        public CommentService(AuctionHouseDbContext context)
        {
            _context = context;
        }

        public async Task AddCommentAsync(int listingId, string userId, string content)
        {
            var comment = new Comment
            {
                ListingId = listingId,
                UserId = userId,
                Content = content
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
        }
    }
}
