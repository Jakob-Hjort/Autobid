using autobid.Domain.Common.Enums;
using autobid.Domain.Services;
using autobid.Domain.Vehicles;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace autobid.Domain.Database
{
	public class CarRepository
	{
		public async Task<int> Add(Vehicle vehicle)
		{
			if (vehicle is Truck truck)
				return (await Add(truck)).VehicleId;

			if (vehicle is Bus bus)
				return (await Add(bus)).VehicleId;

			if (vehicle is PrivatePersonalCar privatePersonal)
				return (await Add(privatePersonal)).VehicleId;

			if (vehicle is ProfessionalPersonalCar professionalPersonalCar)
				return (await Add(professionalPersonalCar)).VehicleId;

			return -1;
		}

		public async Task<(int TruckId, int HeavyVehicleId, int VehicleId)> Add(Truck car)
		{
			string sql = @"INSERT INTO truck([payloadKg], [heavyVehicleId])
				VALUES(@payLoad, @heavyVehicleId);
				SET @NewVehicleId = SCOPE_IDENTITY();
				";

			await using var conn = await Connection.OpenAsync(); // returns SqlConnection
			var ids = await Add(car as HeavyVehicle, conn);

			await using SqlCommand cmd = new(sql, conn);

			var pOut = new SqlParameter("@NewVehicleId", SqlDbType.Int) { Direction = ParameterDirection.Output };

			SqlParameter[] parameters = [
				new("@payLoad" , car.PayloadKg),
				new("@heavyVehicleId", ids.HeavyVehicleId),
				pOut
			];

			cmd.Parameters.AddRange(
				parameters
			);

			await cmd.ExecuteNonQueryAsync();

			return (Convert.ToInt32(pOut.Value), ids.HeavyVehicleId, ids.VehicleId);
		}

		private async Task<int> Add(Fuel fuel, double kmPerLiter, double engineLiters, SqlConnection conn)
		{
			string sql = @"
			INSERT INTO fuelTank(fuel, [kmPerLiter], engineLiters)
			VALUES(@fuel, @kmPerLiter, @engineLiters);
			SET @NewVehicleId = SCOPE_IDENTITY();
			";

			await using var cmd = new SqlCommand(sql, conn);

			var pOut = new SqlParameter("@NewVehicleId", SqlDbType.Int) { Direction = ParameterDirection.Output };

			SqlParameter[] parameters = {
				new SqlParameter("@fuel", SqlDbType.TinyInt) { Value = (int)fuel },
				new SqlParameter("@kmPerLiter", SqlDbType.Float) { Value = kmPerLiter },
				new SqlParameter("@engineLiters", SqlDbType.Float) { Value = engineLiters },
				pOut
			};

			cmd.Parameters.AddRange(parameters);
			await cmd.ExecuteNonQueryAsync();

			return Convert.ToInt32(pOut.Value);
		}

		private async Task<int> Add(Vehicle vehicle, SqlConnection conn)
		{
			int id = await Add(vehicle.Fuel, vehicle.KmPerLiter, vehicle.EngineLiters, conn);
			string sql = @"
			INSERT INTO vehicle([name], distanceTraveledKm, registrationNumber, [year], hasTowHitch, license, energyClass, fuelTankId)
			VALUES(@carName, @DistanceTraveledKm, @RegistrationNumber, @Year, @HasTowHitch, @LicenseType, @Energy, 
			@fuelTankId);
				SET @NewVehicleId = SCOPE_IDENTITY();
			";

			await using var cmd = new SqlCommand(sql, conn);

			var pOut = new SqlParameter("@NewVehicleId", SqlDbType.Int) { Direction = ParameterDirection.Output };

			SqlParameter[] parameters = [
				new("@carName", vehicle.Name),
				new("@DistanceTraveledKm", vehicle.DistanceTraveledKm),
				new("@RegistrationNumber", vehicle.RegistrationNumber),
				new("@Year", vehicle.Year),
				new("@HasTowHitch", Convert.ToInt32(vehicle.HasTowHitch)),
				new("@LicenseType", (int)vehicle.LicenseType),
				new("@Energy", (int)vehicle.Energy),
				new("@fuelTankId", id),
				pOut
			];

			cmd.Parameters.AddRange(parameters);
			await cmd.ExecuteNonQueryAsync();

			return Convert.ToInt32(pOut.Value);
		}

		private async Task<(int VehicleId, int HeavyVehicleId)> Add(HeavyVehicle vehicle, SqlConnection conn)
		{
			int vehicleId = await Add(vehicle as Vehicle, conn);
			var pOut = new SqlParameter("@NewHeavyVehicleId", SqlDbType.Int) { Direction = ParameterDirection.Output };

			string sql = @"
				INSERT INTO heavyVehicle(length, weight, height, vehicleId)
				VALUES(@Length, @Weight, @Height, @VehicleId);
				SET @NewHeavyVehicleId = SCOPE_IDENTITY();
			";
			SqlCommand cmd = new(sql, conn);
			SqlParameter[] parameters = [
				new("@Length", vehicle.Length),
				new("@Weight", vehicle.WeightKg),
				new("@Height", vehicle.HeightMeter),
				new("@VehicleId", vehicleId),
				pOut
			];

			cmd.Parameters.AddRange(
				parameters
			);

			await cmd.ExecuteNonQueryAsync();
			return (vehicleId, Convert.ToInt32(pOut.Value));
		}

		private async Task<(int PersonalCarId, int VehicleId)> Add(PersonalCar vehicle, SqlConnection conn)
		{
			int vehicleId = await Add(vehicle as Vehicle, conn);
			var pOut = new SqlParameter("@NewPersonalCarId", SqlDbType.Int) { Direction = ParameterDirection.Output };

			string sql = @"
				INSERT INTO personalCar(seatsAmount, trunkLength, [trunkWidth], trunkHeight, vehicleId)
				VALUES(@seatsAmount, @trunkLength, @trunkWidth, @trunkHeight, @VehicleId);
				SET @NewPersonalCarId = SCOPE_IDENTITY();
			";
			SqlCommand cmd = new(sql, conn);
			SqlParameter[] parameters = [
				new("@seatsAmount", vehicle.SeatsAmount),
				new("@trunkLength", vehicle.Trunk.L),
				new("@trunkWidth", vehicle.Trunk.W),
				new("@trunkHeight", vehicle.Trunk.H),
				new("@VehicleId", vehicleId),
				pOut
			];

			cmd.Parameters.AddRange(
				parameters
			);

			await cmd.ExecuteNonQueryAsync();
			return (Convert.ToInt32(pOut.Value), vehicleId);
		}
		public async Task<(int ProfessionalPersonalCar, int PersonalCarId, int VehicleId)> Add(ProfessionalPersonalCar car)
		{
			string sql = @"INSERT INTO professionalPersonalCar([hasSafetyBar], [trailerCapacityKg], [personalCarId])
				VALUES(@hasSafetyBar, @trailerCapacity, @personalCarId);
				SET @NewCarId = SCOPE_IDENTITY();";
			await using SqlConnection conn = await Connection.OpenAsync();
			var ids = await Add(car as PersonalCar, conn);
			await using SqlCommand cmd = new(sql, conn);
			var pOut = new SqlParameter("@NewCarId", SqlDbType.Int) { Direction = ParameterDirection.Output };

			SqlParameter[] parameters = [
				new("@hasSafetyBar", car.HasSafetyBar),
				new("@trailerCapacity", car.TrailerCapacityKg),
				new("@personalCarId", ids.PersonalCarId),
				pOut
			];

			cmd.Parameters.AddRange(parameters);

			await cmd.ExecuteNonQueryAsync();

			return (Convert.ToInt32(pOut.Value), ids.PersonalCarId, ids.VehicleId);
		}
		public async Task<(int PrivatePersonalCarPersonalCar, int PersonalCarId, int VehicleId)> Add(PrivatePersonalCar car)
		{
			string sql = @"INSERT INTO privatePersonalCar(hasIsofix, [personalCarId])
				VALUES(@hasIsofix, @personalCarId);
				SET @NewCarId = SCOPE_IDENTITY();";
			await using SqlConnection conn = await Connection.OpenAsync();
			var ids = await Add(car as PersonalCar, conn);
			await using SqlCommand cmd = new(sql, conn);
			var pOut = new SqlParameter("@NewCarId", SqlDbType.Int) { Direction = ParameterDirection.Output };

			SqlParameter[] parameters = [
				new("@hasIsofix", car.HasIsofix),
				new("@personalCarId", ids.PersonalCarId),
				pOut
			];

			cmd.Parameters.AddRange(parameters);

			await cmd.ExecuteNonQueryAsync();

			return (Convert.ToInt32(pOut.Value), ids.PersonalCarId, ids.VehicleId);
		}
		public async Task<(int BusId, int HeavyVehicleId, int VehicleId)> Add(Bus car)
		{
			string sql = @"INSERT INTO bus([seatsAmount], bedsAmount, [hasToilet], [heavyVehicleId])
				VALUES(@seatsAmount, @bedsAmount, @hasToilet, @heavyVehicleId);
				SET @NewVehicleId = SCOPE_IDENTITY();
";

			await using var conn = await Connection.OpenAsync(); // returns SqlConnection
			var ids = await Add(car as HeavyVehicle, conn);

			await using SqlCommand cmd = new(sql, conn);

			var pOut = new SqlParameter("@NewVehicleId", SqlDbType.Int) { Direction = ParameterDirection.Output };

			SqlParameter[] parameters = [
				new("@bedsAmount" , car.BedsAmount),
				new("@seatsAmount" , car.SeatsAmount),
				new("@hasToilet" , car.HasToilet),
				new("@heavyVehicleId", ids.HeavyVehicleId),
				pOut
			];

			cmd.Parameters.AddRange(
				parameters
			);

			await cmd.ExecuteNonQueryAsync();

			return (Convert.ToInt32(pOut.Value), ids.HeavyVehicleId, ids.VehicleId);
		}

		async Task<bool> IsVehicleFound(SqlDataReader reader) =>
			reader.Read() && !await reader.IsDBNullAsync(reader.GetOrdinal("vehicleId"));


		public async Task<Vehicle?> GetSingle(int vehicleId)
		{
			await using var truckConn = await Connection.OpenAsync();
			const string truckViewSql = "SELECT TOP(1) * FROM truckView WHERE vehicleId = @vehicleId";
			const string busViewSql = "SELECT TOP(1) * FROM busView WHERE vehicleId = @vehicleId";
			const string privatePersonalCarViewSql = "SELECT TOP(1) * FROM privatePersonalCarView WHERE vehicleId = @vehicleId";
			const string professionalPersonalCarViewSql = "SELECT TOP(1) * FROM professionalPersonalCarView WHERE vehicleId = @vehicleId";
			await using SqlCommand truckCmd = new(truckViewSql, truckConn);
			truckCmd.Parameters.AddWithValue("@vehicleId", (int)vehicleId);
			var truckReader = await truckCmd.ExecuteReaderAsync();
			if (await IsVehicleFound(truckReader))
			{
				uint truckId = Convert.ToUInt32(truckReader.GetInt32(truckReader.GetOrdinal("truckId")));
				return new Truck(
					truckId,
					truckReader.GetString(truckReader.GetOrdinal("name")),
					truckReader.GetInt32(truckReader.GetOrdinal("distanceTraveledKm")),
					truckReader.GetString(truckReader.GetOrdinal("registrationNumber")),
					truckReader.GetInt16(truckReader.GetOrdinal("year")),
					truckReader.GetDouble(truckReader.GetOrdinal("engineLiters")),
					truckReader.GetBoolean(truckReader.GetOrdinal("hasTowHitch")),
					truckReader.GetDouble(truckReader.GetOrdinal("kmPerLiter"))
				);
			}
            await using var busConn = await Connection.OpenAsync();
            await using SqlCommand busCmd = new(busViewSql, busConn);
			busCmd.Parameters.AddWithValue("@vehicleId", (int)vehicleId);

			var busReader = await busCmd.ExecuteReaderAsync();
			if (await IsVehicleFound(busReader))
			{
				uint truckId = Convert.ToUInt32(busReader.GetInt32(busReader.GetOrdinal("busId")));
				return new Bus(
					truckId,
						busReader.GetString(busReader.GetOrdinal("name")),
						busReader.GetInt32(busReader.GetOrdinal("distanceTraveledKm")),
						busReader.GetString(busReader.GetOrdinal("registrationNumber")),
						busReader.GetInt16(busReader.GetOrdinal("year")),
						busReader.GetDouble(busReader.GetOrdinal("engineLiters")),
						busReader.GetBoolean(busReader.GetOrdinal("hasTowHitch")),
						busReader.GetDouble(busReader.GetOrdinal("kmPerLiter")
					))
				{
					SeatsAmount = busReader.GetInt32(busReader.GetOrdinal("seatsAmount")),
					BedsAmount = busReader.GetInt32(busReader.GetOrdinal("bedsAmount")),
					HasToilet = busReader.GetBoolean(busReader.GetOrdinal("hasToilet"))
				};
			}

			await using SqlConnection privatePersonalCarConn = await Connection.OpenAsync();
            await using SqlCommand privatePersonalCarCmd = new(privatePersonalCarViewSql, privatePersonalCarConn);
			privatePersonalCarCmd.Parameters.AddWithValue("@vehicleId", (int)vehicleId);

			var privatePersonalCarReader = await privatePersonalCarCmd.ExecuteReaderAsync();
			if (await IsVehicleFound(privatePersonalCarReader))
			{
				uint truckId = Convert.ToUInt32(privatePersonalCarReader.GetInt32(privatePersonalCarReader.GetOrdinal("privatePersonalCarId")));

				return new PrivatePersonalCar(
					truckId,
					privatePersonalCarReader.GetString(privatePersonalCarReader.GetOrdinal("name")),
						privatePersonalCarReader.GetInt32(privatePersonalCarReader.GetOrdinal("distanceTraveledKm")),
						privatePersonalCarReader.GetString(privatePersonalCarReader.GetOrdinal("registrationNumber")),
						privatePersonalCarReader.GetInt16(privatePersonalCarReader.GetOrdinal("year")),
						privatePersonalCarReader.GetDouble(privatePersonalCarReader.GetOrdinal("engineLiters")),
						privatePersonalCarReader.GetBoolean(privatePersonalCarReader.GetOrdinal("hasTowHitch")),
						privatePersonalCarReader.GetDouble(privatePersonalCarReader.GetOrdinal("kmPerLiter")
					))
				{
					SeatsAmount = privatePersonalCarReader.GetInt32(privatePersonalCarReader.GetOrdinal("seatsAmount")),
					Trunk = (
						privatePersonalCarReader.GetFloat(privatePersonalCarReader.GetOrdinal("trunkLength")),
						privatePersonalCarReader.GetFloat(privatePersonalCarReader.GetOrdinal("trunkWidth")),
						privatePersonalCarReader.GetFloat(privatePersonalCarReader.GetOrdinal("trunkHeight"))
					),
					HasIsofix = privatePersonalCarReader.GetBoolean(privatePersonalCarReader.GetOrdinal("hasIsofix"))
				};
			}
            await using SqlConnection professionalPersonalCarConn = await Connection.OpenAsync();

            await using SqlCommand professionalPersonalCarCmd = new(professionalPersonalCarViewSql, professionalPersonalCarConn);
			professionalPersonalCarCmd.Parameters.AddWithValue("@vehicleId", (int)vehicleId);

			var professionalPersonalCarReader = await professionalPersonalCarCmd.ExecuteReaderAsync();
			if (await IsVehicleFound(professionalPersonalCarReader))
			{
				uint truckId = Convert.ToUInt32(professionalPersonalCarReader.GetInt32(professionalPersonalCarReader.GetOrdinal("privatePersonalCarId")));

				return new ProfessionalPersonalCar(
					truckId,
					professionalPersonalCarReader.GetString(professionalPersonalCarReader.GetOrdinal("name")),
						professionalPersonalCarReader.GetInt32(professionalPersonalCarReader.GetOrdinal("distanceTraveledKm")),
						professionalPersonalCarReader.GetString(professionalPersonalCarReader.GetOrdinal("registrationNumber")),
						professionalPersonalCarReader.GetInt16(professionalPersonalCarReader.GetOrdinal("year")),
						professionalPersonalCarReader.GetDouble(professionalPersonalCarReader.GetOrdinal("engineLiters")),
						professionalPersonalCarReader.GetBoolean(professionalPersonalCarReader.GetOrdinal("hasTowHitch")),
						professionalPersonalCarReader.GetDouble(professionalPersonalCarReader.GetOrdinal("kmPerLiter")
					))
				{
					SeatsAmount = professionalPersonalCarReader.GetInt32(professionalPersonalCarReader.GetOrdinal("seatsAmount")),
					Trunk = (
						professionalPersonalCarReader.GetFloat(professionalPersonalCarReader.GetOrdinal("trunkLength")),
						professionalPersonalCarReader.GetFloat(professionalPersonalCarReader.GetOrdinal("trunkWidth")),
						professionalPersonalCarReader.GetFloat(professionalPersonalCarReader.GetOrdinal("trunkHeight"))
					),
					HasSafetyBar = professionalPersonalCarReader.GetBoolean(professionalPersonalCarReader.GetOrdinal("hasSafetyBar")),
					TrailerCapacityKg = professionalPersonalCarReader.GetInt32(professionalPersonalCarReader.GetOrdinal("trailerCapacityKg"))
				};
			}

			return null;
		}
	}
}
