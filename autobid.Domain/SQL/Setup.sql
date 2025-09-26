DROP TABLE IF EXISTS professionalPersonalCar
DROP TABLE IF EXISTS privatePersonalCar
DROP TABLE IF EXISTS personalCar
DROP TABLE IF EXISTS truck
DROP TABLE IF EXISTS bus
DROP TABLE IF EXISTS heavyVehicle
DROP TABLE IF EXISTS vehicle
DROP TABLE IF EXISTS fuelTank
DROP TABLE IF EXISTS corporateCustomer
DROP TABLE IF EXISTS privateCustomer
DROP TABLE IF EXISTS [user]



-- vehicles

CREATE TABLE fuelTank(
	fuelTankId INT IDENTITY(1,1) PRIMARY KEY,
	kmPerLiter FLOAT NOT NULL,
	fuel TINYINT NOT NULL
)

CREATE TABLE vehicle(
	vehicleId INT IDENTITY(1,1) PRIMARY KEY,
	[name] NVARCHAR(32) NOT NULL,
	distanceTraveledKm INT NOT NULL,
	registrationNumber NVARCHAR(8) NOT NULL,
	[year] SMALLINT NOT NULL,
	hasTowHitch BIT NOT NULL,
	license TINYINT NOT NULL,
	energyClass TINYINT NOT NULL,
	fuelTankId INT NOT NULL,
	FOREIGN KEY (fuelTankId) REFERENCES fuelTank(fuelTankId)
	ON DELETE CASCADE
)

CREATE TABLE heavyVehicle(
	heavyVehicleId INT IDENTITY(1,1) PRIMARY KEY,
	[length] REAL NOT NULL,
	[weight] REAL NOT NULL,
	height REAL NOT NULL,
	vehicleId INT NOT NULL,
	FOREIGN KEY (vehicleId) REFERENCES vehicle(vehicleId)
	ON DELETE CASCADE
)

CREATE TABLE bus(
	busId INT IDENTITY(1,1) PRIMARY KEY,
	seatsAmount INT NOT NULL,
	bedsAmount INT NOT NULL,
	hasToilet BIT NOT NULL,
	heavyVehicleId INT NOT NULL,
	FOREIGN KEY (heavyVehicleId) REFERENCES heavyVehicle(heavyVehicleId)
	ON DELETE CASCADE
)

CREATE TABLE truck(
	truckId INT IDENTITY(1,1) PRIMARY KEY,
	payloadKg INT NOT NULL,
	heavyVehicleId INT NOT NULL,
	FOREIGN KEY (heavyVehicleId) REFERENCES heavyVehicle(heavyVehicleId)
	ON DELETE CASCADE
)

CREATE TABLE personalCar(
	personalCarId INT IDENTITY(1,1) PRIMARY KEY,
	seatsAmount INT NOT NULL,
	trunkLength REAL NOT NULL,
	trunkWidth REAL NOT NULL,
	trunkHeight REAL NOT NULL,
	vehicleId INT NOT NULL,
	FOREIGN KEY (vehicleId) REFERENCES vehicle(vehicleId)
	ON DELETE CASCADE
) 

CREATE TABLE privatePersonalCar(
	privatePersonalCarId INT IDENTITY(1,1) PRIMARY KEY,
	hasIsofix BIT NOT NULL,
	personalCarId INT NOT NULL,
	FOREIGN KEY (personalCarId) REFERENCES personalCar(personalCarId)
	ON DELETE CASCADE
)

CREATE TABLE professionalPersonalCar(
	professionalPersonalCarId INT IDENTITY(1,1) PRIMARY KEY,
	hasSafetyBar BIT NOT NULL DEFAULT(0),
	trailerCapacityKg INT NOT NULL,
	personalCarId INT NOT NULL,
	FOREIGN KEY (personalCarId) REFERENCES personalCar(personalCarId)
	ON DELETE CASCADE
)

CREATE VIEW busView
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

CREATE VIEW truckView
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

CREATE VIEW privatePersonalCarView
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

CREATE VIEW professionalPersonalCarView
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
	ppc.professionalPersonalCarId,
	ppc.hasSafetyBar,
	ppc.trailerCapacityKg
	FROM vehicle as v
	INNER JOIN fuelTank as f
	ON f.fuelTankId = v.fuelTankId
	INNER JOIN personalCar as pc
	ON pc.vehicleId = v.vehicleId
	INNER JOIN professionalPersonalCar AS ppc
	ON ppc.personalCarId = pc.personalCarId
GO


-- users

CREATE TABLE [user](
	userId INT IDENTITY(1,1) PRIMARY KEY,
	username NVARCHAR(32) NOT NULL,
	passwordHash NVARCHAR(32) NOT NULL,
	balance DECIMAL NOT NULL,
)

CREATE TABLE privateCustomer(
	privateCustomerId INT IDENTITY(1,1) PRIMARY KEY,
	cpr VARCHAR(16) NOT NULL,
	userId INT NOT NULL,
	FOREIGN KEY (userId) REFERENCES [user](userId)
	ON DELETE CASCADE
)

CREATE TABLE corporateCustomer(
	corporateCustomerId INT IDENTITY(1,1) PRIMARY KEY,
	cvr VARCHAR(8) NOT NULL,
	credit DECIMAL NOT NULL,
	userId INT NOT NULL,
	FOREIGN KEY (userId) REFERENCES [user](userId)
	ON DELETE CASCADE
)