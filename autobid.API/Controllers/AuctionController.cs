using System.Text.Json;
using System.Threading.Tasks;
using autobid.Domain.API;
using autobid.Domain.Auctions;
using autobid.Domain.Database.EF;
using autobid.Domain.Users;
using autobid.Domain.Vehicles;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
                using AppDbContext dbContext = new();
                return dbContext.Auctions.ToArray();
            }
            catch
            {
                return [];
            }
        }

        [HttpGet("{id}")]
        public ActionResult<AuctionForAPI> GetAuctionById(int id)
        {
            try
            {
                using AppDbContext dbContext = new();
                
                var auction = dbContext.Auctions.Select(au =>
                    new { 
                        Vehicleid = au.Vehicle.Id,
                        UserId = au.Seller.Id,
                        AuctionId = au.Id,
                        au.CloseDate,
                        au.MinimumPrice
                    }
                ).First(a => a.AuctionId == id);
                User? seller = dbContext.PrivateCustomers.SingleOrDefault(u => u.Id == auction.UserId);
                seller ??= dbContext.CorporateUsers.SingleOrDefault(u => u.Id == auction.AuctionId);

                Vehicle? vehicle = dbContext.Trucks.SingleOrDefault(v => v.Id == auction.Vehicleid);
                vehicle ??= dbContext.Busses.SingleOrDefault(v => v.Id == auction.Vehicleid);
                vehicle ??= dbContext.ProfessionalPersonalCars.SingleOrDefault(v => v.Id == auction.Vehicleid);
                vehicle ??= dbContext.PrivatePersonalCars.SingleOrDefault(v => v.Id == auction.Vehicleid);

                if (vehicle == null)
                    throw new Exception("invalid car type");
                if (seller == null)
                    throw new Exception("invalid user type");  

                Auction auctionForReturn = new(vehicle, seller, auction.MinimumPrice,
                    auction.CloseDate, auction.AuctionId);
                return Ok(AuctionForAPI.FromAuction(auctionForReturn));
            }
            catch(Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("AuctionFromUser/{id}")]
        public IEnumerable<Auction> GetAuctionsFromUser(int userId)
        {
            try
            {
                using AppDbContext dbContext = new();
                return dbContext.Auctions.Where(a => a.Seller.Id == userId).ToArray();
            }
            catch
            {
                return [];
            }
        }

        [HttpPut("CloseAuction")]
        public async Task<ActionResult> CloseAuction([FromBody]
             AuctionForAPI auctionForAPI)
        {
            try
            {
                Auction auction = auctionForAPI.ToAuction();

                using AppDbContext appContext = new();
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
            using AppDbContext appContext = new();
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
        public async Task<ActionResult<AuctionForAPI>> CreateAuction(
            [FromBody] AuctionForAPI auctionForAPI)
        {
            Auction auction = auctionForAPI.ToAuction();

            try
            {
                using AppDbContext appContext = new();
                appContext.Entry(auction.Seller).State = EntityState.Unchanged;
                appContext.Auctions.Add(auction);

                appContext.SaveChanges();
                return Ok(auctionForAPI);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetFromUserId/{id}")]
        public IEnumerable<Auction> GetAuctionsFromUserId(int id)
        {
            try
            {
                using AppDbContext dbContext = new();
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
                using AppDbContext dbContext = new();
                return dbContext.Auctions
                    .Where(a => !a.IsClosed)
                    .Select(a => new AuctionListItem(a.Id, a.Vehicle.Name, a.Vehicle.Year,
                    a.MinimumPrice, a.Seller.Username)).ToArray();
            }
            catch(Exception ex)
            {
                return [];
            }
        }

        [HttpPut("CloseEndedAuctions")]
        public async Task<ActionResult> CloseEndedAuctions()
        {
            try
            {
                using AppDbContext appContext = new();
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
