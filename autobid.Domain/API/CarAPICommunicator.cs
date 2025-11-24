using System;
using System.Net.Http.Json;
using autobid.Domain.Vehicles;

namespace autobid.Domain.API;

public class CarAPICommunicator
{
    const string baseUrl = "http://localhost:5240/api/Car";

    public async Task<IEnumerable<Vehicle>> GetAllVehicles()
    {
        using HttpClient client = new();
        var response = await client.GetAsync($"{baseUrl}/Vehicles");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<IEnumerable<Vehicle>>() ?? [];
        }

        return [];
    }


    public async Task<PrivatePersonalCar?> CreatePrivatePersonalCar(PrivatePersonalCar car)
    {
        using HttpClient client = new();
        var response = await client.PostAsJsonAsync($"{baseUrl}/PrivatePersonalCar", car);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<PrivatePersonalCar?>();
        }
        return null;
    }

    public async Task<ProfessionalPersonalCar?> CreateProfessionalPersonalCar(ProfessionalPersonalCar car)
    {
        using HttpClient client = new();
        var response = await client.PostAsJsonAsync($"{baseUrl}/PrivatePersonalCar", car);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<ProfessionalPersonalCar?>();
        }
        return null;
    }

    public async Task<Truck?> CreateTruck(Truck car)
    {
        using HttpClient client = new();
        var response = await client.PostAsJsonAsync($"{baseUrl}/Truck", car);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Truck?>();
        }
        return null;
    }

    public async Task<Bus?> CreateBus(Bus bus)
    {
        using HttpClient client = new();
        var response = await client.PostAsJsonAsync($"{baseUrl}/Bus", bus);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Bus?>();
        }
        return null;
    }

    public async Task<Vehicle?> DeleteVehicle(int id)
    {
        using HttpClient client = new();
        var response = await client.DeleteAsync($"{baseUrl}/Vehicle/{id}");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Vehicle?>();
        }
        return null;
    }
}
