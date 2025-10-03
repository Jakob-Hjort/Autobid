CREATE OR ALTER VIEW busView
AS
	SELECT 
	v.vehicleId,
	v.[name],
	v.distanceTraveledKm,
	v.registrationNumber,
	v.[year],
	v.hasTowHitch,
	v.license,
	v.energyClass,
	f.kmPerLiter,
	f.fuel,
	f.engineLiters,
	hv.heavyVehicleId,
	hv.[length],
	hv.[weight],
	hv.height,
	b.busId,
	b.seatsAmount,
	b.bedsAmount,
	b.hasToilet
	FROM vehicle as v
	INNER JOIN fuelTank as f
	ON f.fuelTankId = v.fuelTankId
	INNER JOIN heavyVehicle as hv
	ON hv.vehicleId = v.vehicleId
	INNER JOIN bus AS b
	ON b.heavyVehicleId = hv.heavyVehicleId
GO