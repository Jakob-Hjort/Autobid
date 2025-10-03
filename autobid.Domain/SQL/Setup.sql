DROP TABLE IF EXISTS professionalPersonalCar
DROP TABLE IF EXISTS privatePersonalCar
DROP TABLE IF EXISTS personalCar
DROP TABLE IF EXISTS truck
DROP TABLE IF EXISTS bus
DROP TABLE IF EXISTS heavyVehicle
DROP TABLE IF EXISTS corporateCustomer
DROP TABLE IF EXISTS privateCustomer
DROP TABLE IF EXISTS bid
DROP TABLE IF EXISTS auction
DROP TABLE IF EXISTS [user]
DROP TABLE IF EXISTS vehicle
DROP TABLE IF EXISTS fuelTank

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

-- users

CREATE TABLE [user](
	userId INT IDENTITY(1,1) PRIMARY KEY,
	username NVARCHAR(32) NOT NULL,
	passwordHash NVARCHAR(256) NOT NULL,
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

-- auction

CREATE TABLE auction(
	auctionId INT IDENTITY(1,1) PRIMARY KEY,
	minimumPrice DECIMAL NOT NULL,
	isClosed BIT NOT NULL DEFAULT(0),
	vehicleId INT NOT NULL,
	userId INT NOT NULL,
	closeDate DATETIME NOT NULL,
	FOREIGN KEY (userId) REFERENCES [user](userId)
	ON DELETE CASCADE,
	FOREIGN KEY (vehicleId) REFERENCES vehicle(vehicleId)
	ON DELETE CASCADE
)

CREATE TABLE bid(
	bidId INT IDENTITY(1,1) PRIMARY KEY,
	sendTime DATETIME NOT NULL,
	amount DECIMAL NOT NULL,
	userId INT NOT NULL,
	auctionId INT NOT NULL,
	FOREIGN KEY (userId) REFERENCES [user](userId)
	ON DELETE CASCADE,
	FOREIGN KEY (auctionId) REFERENCES auction(auctionId)
)