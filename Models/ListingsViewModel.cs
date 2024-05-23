namespace AuctionHouseApp.Models
{
    public class ListingsViewModel
    {
        public IEnumerable<Listing> Listings { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
