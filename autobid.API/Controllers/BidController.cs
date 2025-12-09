using System.Text.Json;
using autobid.Domain.API;
using autobid.Domain.Auctions;
using autobid.Domain.Database.EF;
using autobid.Domain.Vehicles;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace autobid.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BidController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<Bid> GetAllBids()
        {
            try
            {
                using AppDbContext dbContext = new();
                return dbContext.Bids.ToArray();
            }
            catch
            {
                return [];
            }
        }

        [HttpGet("FromUserId/{id}")]
        public IEnumerable<Bid> GetBidsFromUserId(int id)
        {
            try
            {
                using AppDbContext dbContext = new();
                return dbContext.Bids.Where(b => b.Buyer.Id == id).ToArray();
            }
            catch
            {
                return [];
            }
        }

        [HttpGet("FromAuctionId/{id}")]
        public IEnumerable<Bid> GetBidsFromAuctionId(int id)
        {
            try
            {
                using AppDbContext dbContext = new();
                return dbContext.Bids.Where(b => b.Auction!.Id == id).ToArray();
            }
            catch
            {
                return [];
            }
        }

        [HttpPost]
        public async Task<ActionResult> CreateBid([FromBody] BidForApi bidForAPI)
        {
            try
            {
                using AppDbContext appContext = new();

                Bid bid = bidForAPI.ToBid();
                appContext.Bids.Add(bid);
                appContext.Entry(bid.Buyer).State = EntityState.Unchanged;
                appContext.Entry(bid.Auction).State = EntityState.Unchanged;
                appContext.Entry(bid.Auction.Seller).State = EntityState.Unchanged;
                appContext.Entry(bid.Auction.Vehicle).State = EntityState.Unchanged;
                await appContext.SaveChangesAsync();
                return Ok(bid);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
