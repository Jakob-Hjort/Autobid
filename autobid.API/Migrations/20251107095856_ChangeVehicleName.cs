using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace autobid.API.Migrations
{
    /// <inheritdoc />
    public partial class ChangeVehicleName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Auctions_Vehicle_VehicleId",
                table: "Auctions");

            migrationBuilder.DropForeignKey(
                name: "FK_Busses_Vehicle_Id",
                table: "Busses");

            migrationBuilder.DropForeignKey(
                name: "FK_PrivatePersonalCars_Vehicle_Id",
                table: "PrivatePersonalCars");

            migrationBuilder.DropForeignKey(
                name: "FK_ProfessionalPersonalCars_Vehicle_Id",
                table: "ProfessionalPersonalCars");

            migrationBuilder.DropForeignKey(
                name: "FK_Trucks_Vehicle_Id",
                table: "Trucks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Vehicle",
                table: "Vehicle");

            migrationBuilder.RenameTable(
                name: "Vehicle",
                newName: "Vehicles");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Vehicles",
                table: "Vehicles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Auctions_Vehicles_VehicleId",
                table: "Auctions",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Busses_Vehicles_Id",
                table: "Busses",
                column: "Id",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PrivatePersonalCars_Vehicles_Id",
                table: "PrivatePersonalCars",
                column: "Id",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProfessionalPersonalCars_Vehicles_Id",
                table: "ProfessionalPersonalCars",
                column: "Id",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Trucks_Vehicles_Id",
                table: "Trucks",
                column: "Id",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Auctions_Vehicles_VehicleId",
                table: "Auctions");

            migrationBuilder.DropForeignKey(
                name: "FK_Busses_Vehicles_Id",
                table: "Busses");

            migrationBuilder.DropForeignKey(
                name: "FK_PrivatePersonalCars_Vehicles_Id",
                table: "PrivatePersonalCars");

            migrationBuilder.DropForeignKey(
                name: "FK_ProfessionalPersonalCars_Vehicles_Id",
                table: "ProfessionalPersonalCars");

            migrationBuilder.DropForeignKey(
                name: "FK_Trucks_Vehicles_Id",
                table: "Trucks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Vehicles",
                table: "Vehicles");

            migrationBuilder.RenameTable(
                name: "Vehicles",
                newName: "Vehicle");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Vehicle",
                table: "Vehicle",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Auctions_Vehicle_VehicleId",
                table: "Auctions",
                column: "VehicleId",
                principalTable: "Vehicle",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Busses_Vehicle_Id",
                table: "Busses",
                column: "Id",
                principalTable: "Vehicle",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PrivatePersonalCars_Vehicle_Id",
                table: "PrivatePersonalCars",
                column: "Id",
                principalTable: "Vehicle",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProfessionalPersonalCars_Vehicle_Id",
                table: "ProfessionalPersonalCars",
                column: "Id",
                principalTable: "Vehicle",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Trucks_Vehicle_Id",
                table: "Trucks",
                column: "Id",
                principalTable: "Vehicle",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
