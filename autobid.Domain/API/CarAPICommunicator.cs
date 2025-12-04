using System;
using System.Net.Http.Json;
using autobid.Domain.Vehicles;

namespace autobid.Domain.API;

public class CarAPICommunicator
{
    const string baseUrl = "http://localhost:5240/api/Car";
    readonly CommonApiCommunicatorModules _commonModules = new();
    public async Task<IEnumerable<Vehicle>> GetAllVehicles()
    {
        using HttpClient client = new();
        try
        {
            var response = await client.GetAsync($"{baseUrl}/Vehicles");
            return await _commonModules.ReadJsonIfSucces<IEnumerable<Vehicle>>(response) ?? [];
        }
        catch
        {
            return [];
        }
    }


    public async Task<PrivatePersonalCar?> CreatePrivatePersonalCar(PrivatePersonalCar car)
    {
        using HttpClient client = new();
        try
        {
            var response = await client.PostAsJsonAsync($"{baseUrl}/PrivatePersonalCar", car);
            return await _commonModules.ReadJsonIfSucces<PrivatePersonalCar>(response);
        }
        catch
        {
            return null;
        }
    }

    public async Task<ProfessionalPersonalCar?> CreateProfessionalPersonalCar(ProfessionalPersonalCar car)
    {
        using HttpClient client = new();
        try
        {

            var response = await client.PostAsJsonAsync($"{baseUrl}/PrivatePersonalCar", car);
            return await _commonModules.ReadJsonIfSucces<ProfessionalPersonalCar>(response);
        }
        catch
        {

            return null;
        }
    }

    public async Task<Truck?> CreateTruck(Truck car)
    {
        using HttpClient client = new();
        try
        {

            var response = await client.PostAsJsonAsync($"{baseUrl}/Truck", car);
            return await _commonModules.ReadJsonIfSucces<Truck>(response);
        }
        catch
        {

            return null;
        }
    }

    public async Task<Bus?> CreateBus(Bus bus)
    {
        using HttpClient client = new();
        try
        {

            var response = await client.PostAsJsonAsync($"{baseUrl}/Bus", bus);
            return await _commonModules.ReadJsonIfSucces<Bus>(response);
        }
        catch
        {
            return null;
        }
    }

    public async Task<Vehicle?> DeleteVehicle(int id)
    {
        using HttpClient client = new();
        try
        {
            var response = await client.DeleteAsync($"{baseUrl}/Vehicle/{id}");
            return await _commonModules.ReadJsonIfSucces<Vehicle>(response);
        }
        catch
        {
            return null;
        }
    }
}
