namespace AuctionHouseApp.Models
{
    public class BidsViewModel
    {
        public List<(Listing Listing, decimal BidPrice)> Bids { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
