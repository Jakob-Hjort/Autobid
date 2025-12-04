using System.Text.Json;
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
                var auction = dbContext.Auctions.Single(a => a.Id == id);

                string userJson = auction.Seller switch
                {
                    PrivateCustomer privateCustomer => JsonSerializer.Serialize<PrivateCustomer>(privateCustomer),
                    CorporateCustomer corporateCustomer => JsonSerializer.Serialize<CorporateCustomer>(corporateCustomer),
                    _ => throw new ArgumentException("unknown user type")
                };

                string vehicleJson = auction.Vehicle switch
                {
                    Truck truck => JsonSerializer.Serialize<Truck>(truck),
                    Bus bus => JsonSerializer.Serialize<Bus>(bus),
                    ProfessionalPersonalCar professionalPersonalCar => JsonSerializer
                        .Serialize<ProfessionalPersonalCar>(professionalPersonalCar),
                    PrivatePersonalCar privatePersonalCar => JsonSerializer
                        .Serialize<PrivatePersonalCar>(privatePersonalCar),
                    _ => throw new ArgumentException("unknown vehicle type")
                };
                AuctionForAPI auctionForAPI = new(auction.Id, vehicleJson, 
                    userJson, auction.MinimumPrice, auction.CloseDate, 
                    auction.Vehicle.GetType().Name, auction.Seller.GetType().Name);
                return Ok(auctionForAPI);
            }
            catch
            {
                return NotFound();
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
        public async Task<ActionResult<Auction>> CloseAuction([FromBody]
             AuctionForAPI auctionForAPI)
        {
            try
            {
                Vehicle? vehicle = ConvertJsonToVehicle(auctionForAPI.VehicleType, auctionForAPI.VehicleJson);
                User? user = ConvertJsonToUser(auctionForAPI.SellerType, auctionForAPI.SellerJson);

                Auction auction = new(vehicle!, user!, auctionForAPI.MinPrice, auctionForAPI.CloseDate);

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

        Vehicle? ConvertJsonToVehicle(string TypeName, string json)
        {
            return TypeName switch
            {
                "Truck" => JsonSerializer.Deserialize<Truck>(json),
                "Bus" => JsonSerializer.Deserialize<Bus>(json),
                "ProfessionalPersonalCar" => JsonSerializer.Deserialize<ProfessionalPersonalCar>(json),
                "PrivatePersonalCar" => JsonSerializer.Deserialize<PrivatePersonalCar>(json),
                _ => null
            };
        }

        User? ConvertJsonToUser(string TypeName, string json)
        {
            return TypeName switch
            {
                "CorporateCustomer" => JsonSerializer.Deserialize<CorporateCustomer>(json),
                "PrivateCustomer" => JsonSerializer.Deserialize<PrivateCustomer>(json),
                _ => null
            };
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
        public async Task<ActionResult<Auction>> CreateAuction(
            [FromBody] AuctionForAPI auctionForAPI)
        {
            Vehicle? vehicle = ConvertJsonToVehicle(auctionForAPI.VehicleType, auctionForAPI.VehicleJson);
            User? user = ConvertJsonToUser(auctionForAPI.SellerType, auctionForAPI.SellerJson);
            Auction auction = new(vehicle!, user!, auctionForAPI.MinPrice, auctionForAPI.CloseDate);

            try
            {
                if (vehicle == null || user == null)
                {
                    return BadRequest("Invalid vehicle or user data.");
                }

                using AppDbContext appContext = new();
                appContext.Entry(user).State = EntityState.Unchanged;
                appContext.Auctions.Add(auction);

                appContext.SaveChanges();
                return Ok(auction);
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
