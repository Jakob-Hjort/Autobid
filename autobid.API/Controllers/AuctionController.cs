using autobid.Domain.Auctions;
using autobid.Domain.Database.EF;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace autobid.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuctionController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<Auction> GetAllAuctions()
        {
            try
            {
                AppDbContext dbContext = new();
                return dbContext.Auctions.ToArray();
            }
            catch
            {
                return [];
            }
        }

        [HttpGet("AuctionFromUser/{id}")]
        public IEnumerable<Auction> GetAuctionsFromUser(int userId)
        {
            try
            {
                AppDbContext dbContext = new();
                return dbContext.Auctions.Where(a => a.Seller.Id == userId).ToArray();
            }
            catch
            {
                return [];
            }
        }

        [HttpPut("CloseAuction")]
        public async Task<ActionResult<Auction>> CloseAuction([FromBody] Auction auction)
        {
            try
            {
                AppDbContext appContext = new();
                Bid? highestBid = auction.HighestBid;
                auction.Close();

                if (highestBid != null)
                {
                    highestBid.Buyer.Balance -= highestBid.Amount;
                    auction.Seller.Balance += highestBid.Amount;
                    appContext.Users.Update(highestBid.Buyer);
                    appContext.Users.Update(auction.Seller);
                }


                
                appContext.Auctions.Update(auction);
                await appContext.SaveChangesAsync();
                return Ok(auction);
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
