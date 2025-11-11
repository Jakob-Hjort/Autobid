using autobid.Domain.Auctions;
using autobid.Domain.Database.EF;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
                AppDbContext dbContext = new();
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
                AppDbContext dbContext = new();
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
                AppDbContext dbContext = new();
                return dbContext.Bids.Where(b => b.Auction!.Id == id).ToArray();
            }
            catch
            {
                return [];
            }
        }

        [HttpPost]
        public async Task<ActionResult<Bid>> CreateBid([FromBody] Bid bid)
        {
            try
            {
                AppDbContext appContext = new();
                appContext.Bids.Add(bid);
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
