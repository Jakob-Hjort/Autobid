using System.Threading.Tasks;
using autobid.API.RequestBinders;
using autobid.Domain.Auctions;
using autobid.Domain.Database.EF;
using autobid.Domain.Security;
using autobid.Domain.Users;
using autobid.Domain.Vehicles;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.Identity.Client;

namespace autobid.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    [HttpGet]
    public IEnumerable<User> GetAllUsers()
    {
        try
        {
            using AppDbContext dbContext = new();
            var users = dbContext.CorporateUsers;
            return users.ToArray();
        }
        catch
        {
            return [];
        }

    }

    [HttpPost]
    public async Task<ActionResult<User>> CreateUser([FromBody]
        [ModelBinder(BinderType = typeof(UserRequestBinder))] User user)
    {
        try
        {
            using AppDbContext appDbContext = new();
            if (user is PrivateCustomer privateCustomer)
                appDbContext.PrivateCustomers.Add(privateCustomer);
            if (user is CorporateCustomer corporateCustomer)
                appDbContext.CorporateUsers.Add(corporateCustomer);

            await appDbContext.SaveChangesAsync();
            return Ok(user);
        }
        catch
        {
            return BadRequest();
        }
    }

    [HttpGet("Login")]
    public ActionResult<User> Login([FromQuery] string username, [FromQuery] string password)
    {
        try
        {
            using AppDbContext appContext = new();
            Hasher hasher = new();
            User? user = appContext.CorporateUsers
                .FirstOrDefault(u => u.Username == username );

            user ??= appContext.PrivateCustomers
                .FirstOrDefault(u => u.Username == username );


            return user != null && hasher.Verify(password, user.PasswordHash) ? Ok(user) : NotFound();
        }
        catch
        {
            return NotFound();
        }
    }

    [HttpGet("{id}")]
    public ActionResult<User> GetUserById(int id)
    {
        using AppDbContext appContext = new();
        User? user = appContext.CorporateUsers.Find(id);
        if (user == null)
        {
            return NotFound();
        }
        return user;
    }

    [HttpPost("CorporateCustomer")]
    public ActionResult<User> CreateCorporateCustomer([FromBody] CorporateCustomer user)
    {
        using AppDbContext appContext = new();
        appContext.CorporateUsers.Add(user);
        appContext.SaveChanges();
        return Ok(user);
    }


    [HttpPut("CorporateCustomer/UpdateBalance")]
    public ActionResult UpdateCorporateCustomerBalance([FromBody] CorporateCustomer user, [FromQuery] decimal newBalance)
    {
        try
        {
            using AppDbContext appDbContext = new();
            if (user == null)
            {
                return NotFound();
            }
            user.Balance = newBalance;
            appDbContext.CorporateUsers.Update(user);
            appDbContext.SaveChanges();
            return Ok();
        }
        catch
        {
            return NotFound();
        }
    }

    [HttpPut("CorporateCustomer/UpdatePasswordHash")]
    public ActionResult UpdateCorporateCustomerPasswordHash([FromQuery] int id, [FromQuery] string newPasswordHash)
    {
        try
        {
            using AppDbContext appDbContext = new();
            CorporateCustomer? user = appDbContext.CorporateUsers.Find(id);
            if (user == null)
            {
                return NotFound();
            }
            user.PasswordHash = newPasswordHash;
            appDbContext.CorporateUsers.Update(user);
            appDbContext.SaveChanges();
            return Ok();
        }
        catch
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteUser(int id)
    {
        using AppDbContext appContext = new();

        User? user = appContext.CorporateUsers.Find(id);
        user ??= appContext.PrivateCustomers.Find(id);
        if (user == null)
        {
            return NotFound();
        }
        appContext.Remove(user);
        await appContext.SaveChangesAsync();
        return Ok();
    }

    [HttpPost("PrivateCustomer")]
    public async Task<ActionResult> CreatePrivateCustomer([FromBody] PrivateCustomer user)
    {
        try
        {
            using AppDbContext appContext = new();
            appContext.PrivateCustomers.Add(user);
            await appContext.SaveChangesAsync();
            return Ok();
        }
        catch
        {
            return BadRequest();
        }

    }
    
    [HttpPut("PrivateCustomer/UpdateBalance")]
    public ActionResult UpdatePrivateCustomerBalance([FromQuery] int id, [FromQuery] decimal newBalance)
    {
        try
        {
            using AppDbContext appDbContext = new();
            PrivateCustomer? user = appDbContext.PrivateCustomers.Find(id);
            if (user == null)
            {
                return NotFound();
            }
            user.Balance = newBalance;
            appDbContext.PrivateCustomers.Update(user);
            appDbContext.SaveChanges();
            return Ok();
        }
        catch
        {
            return NotFound();
        }
    }

    [HttpPut("PrivateCustomer/UpdatePasswordHash")]
    public ActionResult UpdatePrivateCustomerPasswordHash([FromQuery] int id, [FromQuery] string newPasswordHash)
    {
        try
        {
            using AppDbContext appDbContext = new();
            PrivateCustomer? user = appDbContext.PrivateCustomers.Find(id);
            if (user == null)
            {
                return NotFound();
            }
            user.PasswordHash = newPasswordHash;
            appDbContext.PrivateCustomers.Update(user);
            appDbContext.SaveChanges();
            return Ok();
        }
        catch
        {
            return NotFound();
        }
    }

    [HttpGet("UserProfileSummary/{userId}")]
    public async Task<ActionResult<UserProfileSummary>> GetUserProfileSummary(uint userId)
    {
        try
        {
            using AppDbContext appDbContext = new();
            User? user = await appDbContext.CorporateUsers.FindAsync(userId);
            user ??= await appDbContext.PrivateCustomers.FindAsync(userId);
            if (user == null)
            {
                return BadRequest();
            }
            IQueryable<Auction> auctions = appDbContext.Auctions.Where(au => au.Seller.Id == user.Id);
            var bids = appDbContext.Bids
                .Where(bid => bid.Buyer.Id == user.Id)
                .OrderByDescending(bid => bid)
                .GroupBy(bid => bid.Auction.Id);
            int auctionCount = auctions.Count();
            int wonAuctionsCount = bids.Count();
            UserProfileSummary userProfile = new UserProfileSummary
                (user.Id, user.Username, user.Balance, auctionCount, wonAuctionsCount);
            return Ok(userProfile);
        }
        catch (Exception ex)
        {
            return BadRequest();
        }
    }

    [HttpGet("DoesUsernameExist/{username}")]
    public async Task<ActionResult<bool>> DoesUsernameExist(string username)
    {
        try
        {
            using AppDbContext appDbContext = new();
            User? user = await appDbContext.PrivateCustomers.
                FirstOrDefaultAsync(u => u.Username == username);
            user ??= await appDbContext.CorporateUsers.
                FirstOrDefaultAsync(u => u.Username == username);
            return Ok(user != null);
        }
        catch
        {
            return BadRequest();
        }
    }


    [HttpGet("GetuserProfileSummary/{userId}")]
    public async Task<ActionResult<UserProfileSummary>> GetuserProfileSummary(int userId)
    {
        try
        {
            using AppDbContext appDbContext = new();
            User? user = await appDbContext.CorporateUsers.FindAsync(userId);
            user ??= await appDbContext.PrivateCustomers.FindAsync(userId);
            if (user == null)
            {
                return BadRequest();
            }

            int wonAuctionsCount = appDbContext.Auctions.Count(
                (au) => Auction.isHighestBidder(au, user));
            int auctionCount = appDbContext.Auctions.Count(au => au.Seller.Id == user.Id);
            UserProfileSummary userProfile = new UserProfileSummary
                (user.Id, user.Username, user.Balance, auctionCount, wonAuctionsCount);
            return Ok(userProfile);
        }
        catch
        {
            return BadRequest();
        }
    }

}
