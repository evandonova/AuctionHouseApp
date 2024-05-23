using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AuctionHouseApp.Data;
using AuctionHouseApp.Models;
using AuctionHouseApp.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using AuctionHouseApp.Services;

namespace AuctionHouseApp.Controllers
{
    public class ListingsController : Controller
    {
        private readonly IListingService _listingService;
        private readonly IBidService _bidService;
        private readonly ICommentService _commentService;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ListingsController(
            IListingService listingService,
            IBidService bidService,
            ICommentService commentService,
            IWebHostEnvironment hostEnvironment)
        {
            _listingService = listingService;
            _bidService = bidService;
            _commentService = commentService;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<IActionResult> Index(string searchString, int page = 1)
        {
            const int pageSize = 3;
            var listings = await _listingService.GetListingsAsync(searchString);
            int totalItems = listings.Count();
            int totalPages = totalItems > 0 ? (int)Math.Ceiling(totalItems / (double)pageSize) : 0;

            page = Math.Max(1, Math.Min(page, totalPages == 0 ? 1 : totalPages));

            var paginatedListings = totalItems > 0 ? listings.Skip((page - 1) * pageSize).Take(pageSize).ToList() : new List<Listing>();

            var viewModel = new ListingsViewModel
            {
                Listings = paginatedListings,
                CurrentPage = page,
                TotalPages = totalPages
            };

            return View(viewModel);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> MyListings(int page = 1)
        {
            const int pageSize = 3;
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var allListings = await _listingService.GetListingsByUserAsync(userId);
            var paginatedListings = allListings.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            int totalItems = allListings.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var viewModel = new ListingsViewModel
            {
                Listings = paginatedListings,
                CurrentPage = page,
                TotalPages = totalPages
            };

            return View("Index", viewModel);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> MyBids(int page = 1)
        {
            const int pageSize = 3;
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var allBids = await _bidService.GetBidsByUserAsync(userId);
            var paginatedBids = allBids.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            int totalItems = allBids.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var viewModel = new BidsViewModel
            {
                Bids = paginatedBids,
                CurrentPage = page,
                TotalPages = totalPages
            };

            return View("MyBids", viewModel);  // Use a dedicated view for displaying bids
        }

        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ListingCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.ImageFile == null || model.ImageFile.Length == 0)
                {
                    return View(model);
                }

                string uniqueFileName = null;
                if (model.ImageFile != null)
                {
                    string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "Images");
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(fileStream);
                    }
                }

                // Create a new Listing object and map the properties
                Listing newListing = new Listing
                {
                    Title = model.Title,
                    Description = model.Description,
                    Price = model.Price,
                    ImagePath = uniqueFileName,
                    IsSold = false,
                    UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                };

                await _listingService.AddListingAsync(newListing);

                return RedirectToAction("Index");
            }

            return View(model);
        }

        // GET: Listings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var listing = await _listingService.GetListingByIdAsync(id.Value);
            if (listing == null)
            {
                return NotFound();
            }

            return View(listing);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddBid(int listingId, string userId, decimal bidAmount)
        {
            await _bidService.AddBidAsync(listingId, userId, bidAmount);
            await _listingService.UpdateListingPriceAsync(listingId, bidAmount);
            return RedirectToAction("Details", new { id = listingId });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CloseBidding(int id)
        {
            await _listingService.CloseBiddingAsync(id);
            return RedirectToAction("Details", new { id = id });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddComment(int listingId, string userId, string content)
        {
            await _commentService.AddCommentAsync(listingId, userId, content);
            return RedirectToAction("Details", new { id = listingId });
        }

        //// GET: Listings/Create
        //public IActionResult Create()
        //{
        //    ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
        //    return View();
        //}

        //// POST: Listings/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("ListingId,Title,Description,Price,ImagePath,IsSold,UserId")] Listing listing)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(listing);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", listing.UserId);
        //    return View(listing);
        //}

        //// GET: Listings/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null || _context.Listings == null)
        //    {
        //        return NotFound();
        //    }

        //    var listing = await _context.Listings.FindAsync(id);
        //    if (listing == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", listing.UserId);
        //    return View(listing);
        //}

        //// POST: Listings/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("ListingId,Title,Description,Price,ImagePath,IsSold,UserId")] Listing listing)
        //{
        //    if (id != listing.ListingId)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(listing);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!ListingExists(listing.ListingId))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", listing.UserId);
        //    return View(listing);
        //}

        //// GET: Listings/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null || _context.Listings == null)
        //    {
        //        return NotFound();
        //    }

        //    var listing = await _context.Listings
        //        .Include(l => l.User)
        //        .FirstOrDefaultAsync(m => m.ListingId == id);
        //    if (listing == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(listing);
        //}

        //// POST: Listings/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    if (_context.Listings == null)
        //    {
        //        return Problem("Entity set 'ApplicationDbContext.Listings'  is null.");
        //    }
        //    var listing = await _context.Listings.FindAsync(id);
        //    if (listing != null)
        //    {
        //        _context.Listings.Remove(listing);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool ListingExists(int id)
        //{
        //  return (_context.Listings?.Any(e => e.ListingId == id)).GetValueOrDefault();
        //}
    }
}
