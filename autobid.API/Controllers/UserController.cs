using autobid.Domain.Database.EF;
using autobid.Domain.Users;
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

    [HttpPost]
    public ActionResult<User> CreateUser([FromBody] CorporateCustomer user)
    {
        AppDbContext appContext = new();
        appContext.CorporateUsers.Add(user);
        appContext.SaveChanges();
        return Ok(user);
    }

    [HttpPut("{id}")]
    public ActionResult UpdateUser(int id, [FromBody] CorporateCustomer updatedUser)
    {
        try
        {
            AppDbContext appDbContext = new();
            appDbContext.CorporateUsers.Update(updatedUser);

            return Ok();
        }
        catch
        {
            return NotFound();
        }
        
    }
}
