using System;
using System.Text.Json.Nodes;
using autobid.Domain.Common.Enums;
using autobid.Domain.Vehicles;

namespace autobid.Domain.Vehicles;

public static class CarConverter
{
    public static Vehicle ConvertToCar(Dictionary<string, JsonNode?> vehicleData, string vehicleType)
    {
        if (!vehicleData.TryGetValue("id", out var idJson) ||
            !vehicleData.TryGetValue("name", out var nameJson) ||
            !vehicleData.TryGetValue("distanceTraveledKm", out var distanceJson) ||
            !vehicleData.TryGetValue("registrationNumber", out var regNoJson) ||
            !vehicleData.TryGetValue("year", out var yearJson) ||
            !vehicleData.TryGetValue("hasTowHitch", out var towHitchJson) ||
            !vehicleData.TryGetValue("licenseType", out var licenseTypeJson) ||
            !vehicleData.TryGetValue("fuel", out var fuelJson) ||
            !vehicleData.TryGetValue("kmPerLiter", out var kmPerLiterJson) ||
            !vehicleData.TryGetValue("engineLiters", out var engineLitersJson))
        {
            throw new ArgumentException("Invalid vehicle data");
        }

        uint vehicleId = idJson?.GetValue<uint>() ?? 0;
        string vehicleName = nameJson?.GetValue<string>() ?? "";
        int distanceKm = distanceJson?.GetValue<int>() ?? 0;
        string registrationNumber = regNoJson?.GetValue<string>() ?? "ll12345";
        int vehicleYear = yearJson?.GetValue<int>() ?? 0;
        bool hasTowHitch = towHitchJson?.GetValue<bool>() ?? false;
        int licenseType = licenseTypeJson?.GetValue<int>() ?? 0;
        Fuel fuel = (Fuel)(fuelJson?.GetValue<int>() ?? 0);
        double kmPerLiter = kmPerLiterJson?.GetValue<double>() ?? 0;
        double engineLiters = engineLitersJson?.GetValue<double>() ?? 0;
        return vehicleType switch
        {
            "Truck" =>
                new Truck(vehicleId, vehicleName, distanceKm, registrationNumber,
                vehicleYear, engineLiters, hasTowHitch, kmPerLiter, fuel),
            "Bus" =>
                new Bus(vehicleId, vehicleName, distanceKm, registrationNumber,
                vehicleYear, engineLiters, hasTowHitch, kmPerLiter, fuel),
            "PrivatePersonalCar" =>
                new PrivatePersonalCar(vehicleId, vehicleName, distanceKm, registrationNumber,
                vehicleYear, engineLiters, hasTowHitch, kmPerLiter, fuel),
            "ProfessionalPersonalCar" =>
                new ProfessionalPersonalCar(vehicleId, vehicleName, distanceKm, registrationNumber,
                vehicleYear, engineLiters, hasTowHitch, kmPerLiter,
                vehicleData["trailerCapacityKg"]?.GetValue<int>() ?? 0, fuel),
            _ => throw new ArgumentException("Invalid vehicle type")
        };
    }
}
