using autobid.Domain.Database.EF;
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

    [HttpGet("Vehicles")]
    public IEnumerable<Vehicle> GetAllVehicles()
    {
        try
        {
            AppDbContext dbContext = new();
            List<Vehicle> vehicles = new(dbContext.Vehicles.Count());
            vehicles.AddRange(dbContext.PrivatePersonalCars);
            vehicles.AddRange(dbContext.ProfessionalPersonalCars);
            vehicles.AddRange(dbContext.Trucks);
            vehicles.AddRange(dbContext.Busses);

            return vehicles.ToArray();
        }
        catch
        {
            return [];
        }
    }

    [HttpGet("PrivatePersonalCars")]
    public IEnumerable<PrivatePersonalCar> GetAllPrivatePersonalCars()
    {
        try
        {
            AppDbContext dbContext = new();
            return dbContext.PrivatePersonalCars.ToArray();
        }
        catch
        {
            return [];
        }
    }

    [HttpGet("ProfessionalPersonalCars")]
    public IEnumerable<ProfessionalPersonalCar> GetAllProfessionalPersonalCars()
    {
        try
        {
            AppDbContext dbContext = new();
            return dbContext.ProfessionalPersonalCars.ToArray();
        }
        catch
        {
            return [];
        }
    }

    [HttpGet("Trucks")]
    public IEnumerable<Truck> GetAllTrucks()
    {
        try
        {
            AppDbContext dbContext = new();
            return dbContext.Trucks.ToArray();
        }
        catch
        {
            return [];
        }
    }

    [HttpGet("Busses")]
    public IEnumerable<Bus> GetAllBusses()
    {
        try
        {
            AppDbContext dbContext = new();
            return dbContext.Busses.ToArray();
        }
        catch
        {
            return [];
        }
    }
    
}
