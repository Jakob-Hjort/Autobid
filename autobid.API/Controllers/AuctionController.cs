using autobid.Domain.Auctions;
using autobid.Domain.Database.EF;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SQLitePCL;

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

                OnAuctionClosedTransferMoney(auction);

                appContext.Auctions.Update(auction);
                await appContext.SaveChangesAsync();
                return Ok(auction);
            }
            catch
            {
                return BadRequest();
            }
        }

        void OnAuctionClosedTransferMoney(Auction auction)
        {
            AppDbContext appContext = new();
            Bid? highestBid = auction.HighestBid;
            if (highestBid != null)
            {
                highestBid.Buyer.Balance -= highestBid.Amount;
                auction.Seller.Balance += highestBid.Amount;
                appContext.Users.Update(highestBid.Buyer);
                appContext.Users.Update(auction.Seller);
            }
        }

        [HttpPost]
        public async Task<ActionResult<Auction>> CreateAuction(
            [ModelBinder(BinderType = typeof(AuctionRequestBinder))][FromBody] Auction auction)
        {
            try
            {
                AppDbContext appContext = new();
                appContext.Auctions.Add(auction);
                await appContext.SaveChangesAsync();
                return Ok(auction);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet("GetFromUserId/{id}")]
        public IEnumerable<Auction> GetAuctionsFromUserId(int id)
        {
            try
            {
                AppDbContext dbContext = new();
                return dbContext.Auctions.Where(a => a.Seller.Id == id).ToArray();
            }
            catch
            {
                return [];
            }
        }

        [HttpGet("OpenListItems")]
        public IEnumerable<AuctionListItem> GetAllAuctonOpenListItems()
        {
            try
            {
                AppDbContext dbContext = new();
                return dbContext.Auctions
                    .Where(a => !a.IsClosed)
                    .Select(a => new AuctionListItem(a.Id, a.Vehicle.Name, a.Vehicle.Year,
                    a.MinimumPrice, a.Seller.Username)).ToArray();
            }
            catch
            {
                return [];
            }
        }

        [HttpPut("CloseEndedAuctions")]
        public async Task<ActionResult> CloseEndedAuctions()
        {
            try
            {
                AppDbContext appContext = new();
                var endedAuctions = appContext.Auctions
                    .Where(a => !a.IsClosed && a.CloseDate <= DateTime.Now).ToList();

                foreach (var auction in endedAuctions)
                {
                    auction.Close();
                    this.OnAuctionClosedTransferMoney(auction);
                    appContext.Auctions.Update(auction);
                }

                await appContext.SaveChangesAsync();
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
