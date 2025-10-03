using autobid.Domain.Common;
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
				return await Add(truck);

			if (vehicle is Bus bus)
				return await Add(bus);

			if (vehicle is PrivatePersonalCar privatePersonal)
				return await Add(privatePersonal);

			if (vehicle is ProfessionalPersonalCar professionalPersonalCar)
				return await Add(professionalPersonalCar);

			return -1;
		}

		public async Task<int> Add(Truck car)
		{
			string sql = @"INSERT INTO truck([payloadKg], [heavyVehicleId])
				VALUES(@payLoad, @heavyVehicleId);
				SET @NewVehicleId = SCOPE_IDENTITY();
				";

			await using var conn = await Connection.OpenAsync(); // returns SqlConnection
			int id = await Add(car as HeavyVehicle, conn);

			await using SqlCommand cmd = new(sql, conn);

			var pOut = new SqlParameter("@NewVehicleId", SqlDbType.Int) { Direction = ParameterDirection.Output };

			SqlParameter[] parameters = [
				new("@payLoad" , car.PayloadKg),
				new("@heavyVehicleId", id),
				pOut
			];

			cmd.Parameters.AddRange(
				parameters
			);

			await cmd.ExecuteNonQueryAsync();

			return Convert.ToInt32(pOut.Value);
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

		private async Task<int> Add(HeavyVehicle vehicle, SqlConnection conn)
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
			return Convert.ToInt32(pOut.Value);
		}

		private async Task<int> Add(PersonalCar vehicle, SqlConnection conn)
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
			return Convert.ToInt32(pOut.Value);
		}
		public async Task<int> Add(ProfessionalPersonalCar car)
		{
			string sql = @"INSERT INTO professionalPersonalCar([hasSafetyBar], [trailerCapacityKg], [personalCarId])
				VALUES(@hasSafetyBar, @trailerCapacity, @personalCarId);
				SET @NewCarId = SCOPE_IDENTITY();";
			await using SqlConnection conn = await Connection.OpenAsync();
			int vehicleId = await Add(car as PersonalCar, conn);
			await using SqlCommand cmd = new(sql, conn);
			var pOut = new SqlParameter("@NewCarId", SqlDbType.Int) { Direction = ParameterDirection.Output };

			SqlParameter[] parameters = [
				new("@hasSafetyBar", car.HasSafetyBar),
				new("@trailerCapacity", car.TrailerCapacityKg),
				new("@personalCarId", vehicleId),
				pOut
			];

			cmd.Parameters.AddRange(parameters);

			await cmd.ExecuteNonQueryAsync();

			return Convert.ToInt32(pOut.Value);
		}
		public async Task<int> Add(PrivatePersonalCar car)
		{
			string sql = @"INSERT INTO privatePersonalCar(hasIsofix, [personalCarId])
				VALUES(@hasIsofix, @personalCarId);
				SET @NewCarId = SCOPE_IDENTITY();";
			await using SqlConnection conn = await Connection.OpenAsync();
			int vehicleId = await Add(car as PersonalCar, conn);
			await using SqlCommand cmd = new(sql, conn);
			var pOut = new SqlParameter("@NewCarId", SqlDbType.Int) { Direction = ParameterDirection.Output };

			SqlParameter[] parameters = [
				new("@hasIsofix", car.HasIsofix),
				new("@personalCarId", vehicleId),
				pOut
			];

			cmd.Parameters.AddRange(parameters);

			await cmd.ExecuteNonQueryAsync();

			return Convert.ToInt32(pOut.Value);
		}
		public async Task<int> Add(Bus car)
		{
			string sql = @"INSERT INTO bus([seatsAmount], bedsAmount, [hasToilet], [heavyVehicleId])
				VALUES(@seatsAmount, @bedsAmount, @hasToilet, @heavyVehicleId);
				SET @NewVehicleId = SCOPE_IDENTITY();
";

			await using var conn = await Connection.OpenAsync(); // returns SqlConnection
			int id = await Add(car as HeavyVehicle, conn);

			await using SqlCommand cmd = new(sql, conn);

			var pOut = new SqlParameter("@NewVehicleId", SqlDbType.Int) { Direction = ParameterDirection.Output };

			SqlParameter[] parameters = [
				new("@bedsAmount" , car.BedsAmount),
				new("@seatsAmount" , car.SeatsAmount),
				new("@hasToilet" , car.HasToilet),
				new("@heavyVehicleId", id),
				pOut
			];

			cmd.Parameters.AddRange(
				parameters
			);

			await cmd.ExecuteNonQueryAsync();

			return Convert.ToInt32(pOut.Value);
		}

		async Task<bool> IsVehicleFound(SqlDataReader reader) =>
			reader.Read() && !await reader.IsDBNullAsync(reader.GetOrdinal("vehicleId"));


		public async Task<Vehicle?> GetSingle(int vehicleId)
		{
			await using var conn = await Connection.OpenAsync();
			const string truckViewSql = "SELECT TOP(1) * FROM truckView WHERE vehicleId = @vehicleId";
			const string busViewSql = "SELECT TOP(1) * FROM busView WHERE vehicleId = @vehicleId";
			const string privatePersonalCarViewSql = "SELECT TOP(1) * FROM privatePersonalCarView WHERE vehicleId = @vehicleId";
			const string professionalPersonalCarViewSql = "SELECT TOP(1) * FROM professionalPersonalCarView WHERE vehicleId = @vehicleId";
			await using SqlCommand truckCmd = new(truckViewSql, conn);
			truckCmd.Parameters.AddWithValue("@vehicleId", vehicleId);
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
					truckReader.GetDouble(truckReader.GetOrdinal("kmPerLiter")),
					truckReader.GetBoolean(truckReader.GetOrdinal("hasTowHitch")),
					truckReader.GetDouble(truckReader.GetOrdinal("kmPerLiter"))
				);
			}
			await using SqlCommand busCmd = new(busViewSql, conn);
			busCmd.Parameters.AddWithValue("@vehicleId", vehicleId);

			var busReader = await busCmd.ExecuteReaderAsync();
			if (await IsVehicleFound(busReader))
			{
				uint truckId = Convert.ToUInt32(busReader.GetInt32(busReader.GetOrdinal("busId")));
				return new Bus(
					truckId,
						busReader.GetString(busReader.GetOrdinal("name")),
						busReader.GetInt32(busReader.GetOrdinal("distanceTraveledKm")),
						busReader.GetString(busReader.GetOrdinal("registrationNumber")),
						busReader.GetInt32(busReader.GetOrdinal("year")),
						busReader.GetDouble(busReader.GetOrdinal("kmPerLiter")),
						busReader.GetBoolean(busReader.GetOrdinal("hasTowHitch")),
						busReader.GetDouble(busReader.GetOrdinal("kmPerLiter")
					))
				{
					SeatsAmount = busReader.GetInt32(busReader.GetOrdinal("seatsAmount")),
					BedsAmount = busReader.GetInt32(busReader.GetOrdinal("bedsAmount")),
					HasToilet = busReader.GetBoolean(busReader.GetOrdinal("hasToilet"))
				};
			}
			await using SqlCommand privatePersonalCarCmd = new(privatePersonalCarViewSql, conn);
			privatePersonalCarCmd.Parameters.AddWithValue("@vehicleId", vehicleId);

			var privatePersonalCarReader = await privatePersonalCarCmd.ExecuteReaderAsync();
			if (await IsVehicleFound(privatePersonalCarReader))
			{
				uint truckId = Convert.ToUInt32(privatePersonalCarReader.GetInt32(privatePersonalCarReader.GetOrdinal("privatePersonalCarId")));

				return new PrivatePersonalCar(
					truckId,
					privatePersonalCarReader.GetString(privatePersonalCarReader.GetOrdinal("name")),
						privatePersonalCarReader.GetInt32(privatePersonalCarReader.GetOrdinal("distanceTraveledKm")),
						privatePersonalCarReader.GetString(privatePersonalCarReader.GetOrdinal("registrationNumber")),
						privatePersonalCarReader.GetInt32(privatePersonalCarReader.GetOrdinal("year")),
						privatePersonalCarReader.GetDouble(privatePersonalCarReader.GetOrdinal("kmPerLiter")),
						privatePersonalCarReader.GetBoolean(privatePersonalCarReader.GetOrdinal("hasTowHitch")),
						privatePersonalCarReader.GetDouble(privatePersonalCarReader.GetOrdinal("kmPerLiter")
					))
				{
					SeatsAmount = privatePersonalCarReader.GetInt32(privatePersonalCarReader.GetOrdinal("seatsAmount")),
					Trunk = (
						privatePersonalCarReader.GetDouble(privatePersonalCarReader.GetOrdinal("trunkLength")),
						privatePersonalCarReader.GetDouble(privatePersonalCarReader.GetOrdinal("trunkWidth")),
						privatePersonalCarReader.GetDouble(privatePersonalCarReader.GetOrdinal("trunkHeight"))
					),
					HasIsofix = privatePersonalCarReader.GetBoolean(privatePersonalCarReader.GetOrdinal("hasIsofix"))
				};
			}
			await using SqlCommand professionalPersonalCarCmd = new(professionalPersonalCarViewSql, conn);
			professionalPersonalCarCmd.Parameters.AddWithValue("@vehicleId", vehicleId);

			var professionalPersonalCarReader = await professionalPersonalCarCmd.ExecuteReaderAsync();
			if (await IsVehicleFound(professionalPersonalCarReader))
			{
				uint truckId = Convert.ToUInt32(professionalPersonalCarReader.GetInt32(professionalPersonalCarReader.GetOrdinal("privatePersonalCarId")));

				return new ProfessionalPersonalCar(
					truckId,
					professionalPersonalCarReader.GetString(professionalPersonalCarReader.GetOrdinal("name")),
						professionalPersonalCarReader.GetInt32(professionalPersonalCarReader.GetOrdinal("distanceTraveledKm")),
						professionalPersonalCarReader.GetString(professionalPersonalCarReader.GetOrdinal("registrationNumber")),
						professionalPersonalCarReader.GetInt32(professionalPersonalCarReader.GetOrdinal("year")),
						professionalPersonalCarReader.GetDouble(professionalPersonalCarReader.GetOrdinal("kmPerLiter")),
						professionalPersonalCarReader.GetBoolean(professionalPersonalCarReader.GetOrdinal("hasTowHitch")),
						professionalPersonalCarReader.GetDouble(professionalPersonalCarReader.GetOrdinal("kmPerLiter")
					))
				{
					SeatsAmount = professionalPersonalCarReader.GetInt32(professionalPersonalCarReader.GetOrdinal("seatsAmount")),
					Trunk = (
						professionalPersonalCarReader.GetDouble(professionalPersonalCarReader.GetOrdinal("trunkLength")),
						professionalPersonalCarReader.GetDouble(professionalPersonalCarReader.GetOrdinal("trunkWidth")),
						professionalPersonalCarReader.GetDouble(professionalPersonalCarReader.GetOrdinal("trunkHeight"))
					),
					HasSafetyBar = professionalPersonalCarReader.GetBoolean(professionalPersonalCarReader.GetOrdinal("hasSafetyBar")),
					TrailerCapacityKg = professionalPersonalCarReader.GetInt32(professionalPersonalCarReader.GetOrdinal("trailerCapacityKg"))
				};
			}

			return null;
		}
	}
}
