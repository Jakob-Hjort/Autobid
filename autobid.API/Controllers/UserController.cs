using autobid.Domain.Database.EF;
using autobid.Domain.Security;
using autobid.Domain.Users;
using autobid.Domain.Vehicles;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
            AppDbContext dbContext = new();
            var users = dbContext.CorporateUsers;
            return users.ToArray();
        }
        catch (Exception ex)
        {
            return [];
        }

    }

    [HttpGet("Login")]
    public ActionResult<User> Login([FromQuery] string username, [FromQuery] string password)
    {
        try
        {
            AppDbContext appContext = new();
            Hasher hasher = new();
            User? user = appContext.CorporateUsers
                .FirstOrDefault(u => u.Username == username && hasher.Verify(password, u.PasswordHash));

            user ??= appContext.PrivateCustomers
                .FirstOrDefault(u => u.Username == username && hasher.Verify(password, u.PasswordHash));

            return user != null ? Ok(user) : NotFound();
        }
        catch
        {
            return NotFound();
        }
    }

    [HttpGet("{id}")]
    public ActionResult<User> GetUserById(int id)
    {
        AppDbContext appContext = new();
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
        AppDbContext appContext = new();
        appContext.CorporateUsers.Add(user);
        appContext.SaveChanges();
        return Ok(user);
    }

    [HttpPut("CorporateCustomer/UpdateBalance")]
    public ActionResult UpdateCorporateCustomerBalance([FromQuery] int id, [FromQuery] decimal newBalance)
    {
        try
        {
            AppDbContext appDbContext = new();
            CorporateCustomer? user = appDbContext.CorporateUsers.Find(id);
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
            AppDbContext appDbContext = new();
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
        AppDbContext appContext = new();
        
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
    public ActionResult CreatePrivateCustomer([FromBody] PrivateCustomer user)
    {
        try
        {
            AppDbContext appContext = new();
            appContext.PrivateCustomers.Add(user);
            appContext.SaveChanges();
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
            AppDbContext appDbContext = new();
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
            AppDbContext appDbContext = new();
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

}
