using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace autobid.API.Migrations
{
    /// <inheritdoc />
    public partial class changedNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommercialVehicles_Size_TrunkId",
                table: "CommercialVehicles");

            migrationBuilder.DropForeignKey(
                name: "FK_CommercialVehicles_Vehicle_Id",
                table: "CommercialVehicles");

            migrationBuilder.DropForeignKey(
                name: "FK_PrivateVehicles_Size_TrunkId",
                table: "PrivateVehicles");

            migrationBuilder.DropForeignKey(
                name: "FK_PrivateVehicles_Vehicle_Id",
                table: "PrivateVehicles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PrivateVehicles",
                table: "PrivateVehicles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CommercialVehicles",
                table: "CommercialVehicles");

            migrationBuilder.RenameTable(
                name: "PrivateVehicles",
                newName: "PrivatePersonalCars");

            migrationBuilder.RenameTable(
                name: "CommercialVehicles",
                newName: "ProfessionalPersonalCars");

            migrationBuilder.RenameIndex(
                name: "IX_PrivateVehicles_TrunkId",
                table: "PrivatePersonalCars",
                newName: "IX_PrivatePersonalCars_TrunkId");

            migrationBuilder.RenameIndex(
                name: "IX_CommercialVehicles_TrunkId",
                table: "ProfessionalPersonalCars",
                newName: "IX_ProfessionalPersonalCars_TrunkId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PrivatePersonalCars",
                table: "PrivatePersonalCars",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProfessionalPersonalCars",
                table: "ProfessionalPersonalCars",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PrivatePersonalCars_Size_TrunkId",
                table: "PrivatePersonalCars",
                column: "TrunkId",
                principalTable: "Size",
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
                name: "FK_ProfessionalPersonalCars_Size_TrunkId",
                table: "ProfessionalPersonalCars",
                column: "TrunkId",
                principalTable: "Size",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProfessionalPersonalCars_Vehicle_Id",
                table: "ProfessionalPersonalCars",
                column: "Id",
                principalTable: "Vehicle",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PrivatePersonalCars_Size_TrunkId",
                table: "PrivatePersonalCars");

            migrationBuilder.DropForeignKey(
                name: "FK_PrivatePersonalCars_Vehicle_Id",
                table: "PrivatePersonalCars");

            migrationBuilder.DropForeignKey(
                name: "FK_ProfessionalPersonalCars_Size_TrunkId",
                table: "ProfessionalPersonalCars");

            migrationBuilder.DropForeignKey(
                name: "FK_ProfessionalPersonalCars_Vehicle_Id",
                table: "ProfessionalPersonalCars");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProfessionalPersonalCars",
                table: "ProfessionalPersonalCars");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PrivatePersonalCars",
                table: "PrivatePersonalCars");

            migrationBuilder.RenameTable(
                name: "ProfessionalPersonalCars",
                newName: "CommercialVehicles");

            migrationBuilder.RenameTable(
                name: "PrivatePersonalCars",
                newName: "PrivateVehicles");

            migrationBuilder.RenameIndex(
                name: "IX_ProfessionalPersonalCars_TrunkId",
                table: "CommercialVehicles",
                newName: "IX_CommercialVehicles_TrunkId");

            migrationBuilder.RenameIndex(
                name: "IX_PrivatePersonalCars_TrunkId",
                table: "PrivateVehicles",
                newName: "IX_PrivateVehicles_TrunkId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CommercialVehicles",
                table: "CommercialVehicles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PrivateVehicles",
                table: "PrivateVehicles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CommercialVehicles_Size_TrunkId",
                table: "CommercialVehicles",
                column: "TrunkId",
                principalTable: "Size",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CommercialVehicles_Vehicle_Id",
                table: "CommercialVehicles",
                column: "Id",
                principalTable: "Vehicle",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PrivateVehicles_Size_TrunkId",
                table: "PrivateVehicles",
                column: "TrunkId",
                principalTable: "Size",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PrivateVehicles_Vehicle_Id",
                table: "PrivateVehicles",
                column: "Id",
                principalTable: "Vehicle",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
