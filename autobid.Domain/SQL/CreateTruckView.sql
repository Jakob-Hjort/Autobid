CREATE OR ALTER VIEW truckView
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
	hv.heavyVehicleId,
	hv.[length],
	hv.[weight],
	hv.height,
	tr.truckId,
	tr.payloadKg
	FROM vehicle as v
	INNER JOIN fuelTank as f
	ON f.fuelTankId = v.fuelTankId
	INNER JOIN heavyVehicle as hv
	ON hv.vehicleId = v.vehicleId
	INNER JOIN truck AS tr
	ON tr.heavyVehicleId = hv.heavyVehicleId
GO
