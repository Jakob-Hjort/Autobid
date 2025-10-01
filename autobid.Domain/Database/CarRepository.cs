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

		private async Task<int> Add(Fuel fuel, double kmPerLiter, SqlConnection conn)
		{
			string sql = @"
			INSERT INTO fuelTank(fuel, [kmPerLiter])
			VALUES(@fuel, @kmPerLiter);
			SET @NewVehicleId = SCOPE_IDENTITY();
			";

			await using var cmd = new SqlCommand(sql, conn);

			var pOut = new SqlParameter("@NewVehicleId", SqlDbType.Int) { Direction = ParameterDirection.Output };

			SqlParameter[] parameters = {
				new SqlParameter("@fuel", SqlDbType.TinyInt) { Value = (int)fuel },
				new SqlParameter("@kmPerLiter", SqlDbType.Float) { Value = (float)kmPerLiter },
				pOut
			};

			cmd.Parameters.AddRange(parameters);
			await cmd.ExecuteNonQueryAsync();

			return Convert.ToInt32(pOut.Value);
		}

		private async Task<int> Add(Vehicle vehicle, SqlConnection conn)
		{
			int id = await Add(vehicle.Fuel, vehicle.KmPerLiter, conn);
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
			return Convert.ToInt32(pOut.SqlValue);
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

			await cmd.ExecuteNonQueryAsync();

			return Convert.ToInt32(pOut.SqlValue);
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

			await cmd.ExecuteNonQueryAsync();

			return Convert.ToInt32(pOut.SqlValue);
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
	}
}
