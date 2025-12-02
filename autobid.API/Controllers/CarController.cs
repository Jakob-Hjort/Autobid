using System.Threading.Tasks;
using autobid.Domain.Database.EF;
using autobid.Domain.Vehicles;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace autobid.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarController : ControllerBase
    {
        [HttpGet("Vehicles")]
        public IEnumerable<Vehicle> GetAllVehicles()
        {
            try
            {
                using AppDbContext dbContext = new();
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
                using AppDbContext dbContext = new();
                return dbContext.PrivatePersonalCars.ToArray();
            }
            catch
            {
                return [];
            }
        }

        [HttpPost("PrivatePersonalCar")]
        public async Task<ActionResult<PrivatePersonalCar>> CreatePrivatePersonalCar([FromBody] PrivatePersonalCar car)
        {
            using AppDbContext appContext = new();
            appContext.PrivatePersonalCars.Add(car);
            await appContext.SaveChangesAsync();
            return Ok(car);
        }

        [HttpPut("PrivatePersonalCar")]
        public async Task<ActionResult<ProfessionalPersonalCar>> CreatePrivatePersonalCar([FromBody] ProfessionalPersonalCar car)
        {
            using AppDbContext appContext = new();
            appContext.ProfessionalPersonalCars.Update(car);
            await appContext.SaveChangesAsync();
            return Ok(car);
        }

        [HttpDelete("PrivatePersonalCar/{id}")]
        public async Task<ActionResult> DeletePrivatePersonalCar(int id)
        {
            using AppDbContext appContext = new();
            PrivatePersonalCar? car = await appContext.PrivatePersonalCars.FindAsync(id);
            if (car == null)
            {
                return NotFound();
            }
            appContext.PrivatePersonalCars.Remove(car);
            await appContext.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("PrivatePersonalCar")]
        public async Task<ActionResult> DeleteProfessionalPersonalCar([FromBody] PrivatePersonalCar car)
        {
            using AppDbContext appContext = new();
            appContext.PrivatePersonalCars.Remove(car);
            await appContext.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("ProfessionalPersonalCars")]
        public IEnumerable<ProfessionalPersonalCar> GetAllProfessionalPersonalCars()
        {
            try
            {
                using AppDbContext dbContext = new();
                return dbContext.ProfessionalPersonalCars.ToArray();
            }
            catch
            {
                return [];
            }
        }

        [HttpPost("ProfessionalPersonalCar")]
        public async Task<ActionResult<ProfessionalPersonalCar>> CreateProfessionalPersonalCar([FromBody] ProfessionalPersonalCar ProfessionalPersonal)
        {
            using AppDbContext appContext = new();
            appContext.ProfessionalPersonalCars.Add(ProfessionalPersonal);
            await appContext.SaveChangesAsync();
            return Ok(ProfessionalPersonal);
        }

        [HttpPut("ProfessionalPersonalCar")]
        public async Task<ActionResult> UpdateProfessionalPersonalCar([FromBody] ProfessionalPersonalCar updatedCar)
        {
            try
            {
                using AppDbContext appDbContext = new();
                appDbContext.ProfessionalPersonalCars.Update(updatedCar);
                await appDbContext.SaveChangesAsync();
                return Ok();
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpDelete("ProfessionalPersonalCar/{id}")]
        public async Task<ActionResult> DeleteProfessionalPersonalCar(int id)
        {
            using AppDbContext appContext = new();
            ProfessionalPersonalCar? car = appContext.ProfessionalPersonalCars.Find(id);
            if (car == null)
            {
                return NotFound();
            }
            appContext.ProfessionalPersonalCars.Remove(car);
            await appContext.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("ProfessionalPersonalCar")]
        public async Task<ActionResult> DeleteProfessionalPersonalCar([FromBody] ProfessionalPersonalCar car)
        {
            using AppDbContext appContext = new();
            appContext.ProfessionalPersonalCars.Remove(car);
            await appContext.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("Trucks")]
        public IEnumerable<Truck> GetAllTrucks()
        {
            try
            {
                using AppDbContext dbContext = new();
                return dbContext.Trucks.ToArray();
            }
            catch
            {
                return [];
            }
        }

        [HttpPost("Truck")]
        public async Task<ActionResult<Truck>> CreateTruck([FromBody] Truck truck)
        {
            using AppDbContext appContext = new();
            appContext.Trucks.Add(truck);
            await appContext.SaveChangesAsync();
            return Ok(truck);
        }

        [HttpPut("Truck")]
        public async Task<ActionResult> UpdateTruck([FromBody] Truck updatedTruck)
        {
            try
            {
                using AppDbContext appDbContext = new();
                appDbContext.Trucks.Update(updatedTruck);
                await appDbContext.SaveChangesAsync();
                return Ok();
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpDelete("Truck/{id}")]
        public async Task<ActionResult> DeleteTruck(int id)
        {
            using AppDbContext appContext = new();
            Truck? truck = appContext.Trucks.Find(id);
            if (truck == null)
            {
                return NotFound();
            }
            appContext.Trucks.Remove(truck);
            await appContext.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("Truck")]
        public async Task<ActionResult> DeleteTruck([FromBody] Truck truck)
        {
            using AppDbContext appContext = new();
            appContext.Trucks.Remove(truck);
            await appContext.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("Busses")]
        public IEnumerable<Bus> GetAllBusses()
        {
            try
            {
                using AppDbContext dbContext = new();
                return dbContext.Busses.ToArray();
            }
            catch
            {
                return [];
            }
        }

        [HttpPost("Bus")]
        public async Task<ActionResult<Bus>> CreateBus([FromBody] Bus bus)
        {
            using AppDbContext appContext = new();
            appContext.Busses.Add(bus);
            await appContext.SaveChangesAsync();
            return Ok(bus);
        }

        [HttpPut("Bus")]
        public async Task<ActionResult> UpdateBus([FromBody] Bus updatedBus)
        {
            try
            {
                using AppDbContext appDbContext = new();
                appDbContext.Busses.Update(updatedBus);
                await appDbContext.SaveChangesAsync();
                return Ok();
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpDelete("Bus/{id}")]
        public async Task<ActionResult> DeleteBus(int id)
        {
            using AppDbContext appContext = new();
            Bus? bus = appContext.Busses.Find(id);
            if (bus == null)
            {
                return NotFound();
            }
            appContext.Busses.Remove(bus);
            await appContext.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("Bus")]
        public async Task<ActionResult> DeleteBus([FromBody] Bus bus)
        {
            using AppDbContext appContext = new();
            appContext.Busses.Remove(bus);
            await appContext.SaveChangesAsync();
            return Ok();
        }
    }
}
