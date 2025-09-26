CREATE OR ALTER VIEW privatePersonalCarView
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
	pc.personalCarId,
	pc.seatsAmount,
	pc.trunkHeight,
	pc.trunkLength,
	pc.trunkWidth,
	ppc.privatePersonalCarId,
	ppc.hasIsofix
	FROM vehicle as v
	INNER JOIN fuelTank as f
	ON f.fuelTankId = v.fuelTankId
	INNER JOIN personalCar as pc
	ON pc.vehicleId = v.vehicleId
	INNER JOIN privatePersonalCar AS ppc
	ON ppc.personalCarId = pc.personalCarId
GO