using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace autobid.API.Migrations
{
    /// <inheritdoc />
    public partial class addedCars : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Busses",
                columns: table => new
                {
                    Id = table.Column<uint>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SeatsAmount = table.Column<int>(type: "INTEGER", nullable: false),
                    BedsAmount = table.Column<int>(type: "INTEGER", nullable: false),
                    HasToilet = table.Column<bool>(type: "INTEGER", nullable: false),
                    HeightMeter = table.Column<double>(type: "REAL", nullable: false),
                    WeightKg = table.Column<double>(type: "REAL", nullable: false),
                    Length = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Busses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Busses_Vehicle_Id",
                        column: x => x.Id,
                        principalTable: "Vehicle",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Size",
                columns: table => new
                {
                    Id = table.Column<uint>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    W = table.Column<double>(type: "REAL", nullable: false),
                    H = table.Column<double>(type: "REAL", nullable: false),
                    L = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Size", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Trucks",
                columns: table => new
                {
                    Id = table.Column<uint>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PayloadKg = table.Column<int>(type: "INTEGER", nullable: false),
                    HeightMeter = table.Column<double>(type: "REAL", nullable: false),
                    WeightKg = table.Column<double>(type: "REAL", nullable: false),
                    Length = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trucks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Trucks_Vehicle_Id",
                        column: x => x.Id,
                        principalTable: "Vehicle",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommercialVehicles",
                columns: table => new
                {
                    Id = table.Column<uint>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    HasSafetyBar = table.Column<bool>(type: "INTEGER", nullable: false),
                    TrailerCapacityKg = table.Column<int>(type: "INTEGER", nullable: false),
                    SeatsAmount = table.Column<int>(type: "INTEGER", nullable: false),
                    TrunkId = table.Column<uint>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommercialVehicles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommercialVehicles_Size_TrunkId",
                        column: x => x.TrunkId,
                        principalTable: "Size",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommercialVehicles_Vehicle_Id",
                        column: x => x.Id,
                        principalTable: "Vehicle",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PrivateVehicles",
                columns: table => new
                {
                    Id = table.Column<uint>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    HasIsofix = table.Column<bool>(type: "INTEGER", nullable: false),
                    SeatsAmount = table.Column<int>(type: "INTEGER", nullable: false),
                    TrunkId = table.Column<uint>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrivateVehicles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrivateVehicles_Size_TrunkId",
                        column: x => x.TrunkId,
                        principalTable: "Size",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PrivateVehicles_Vehicle_Id",
                        column: x => x.Id,
                        principalTable: "Vehicle",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommercialVehicles_TrunkId",
                table: "CommercialVehicles",
                column: "TrunkId");

            migrationBuilder.CreateIndex(
                name: "IX_PrivateVehicles_TrunkId",
                table: "PrivateVehicles",
                column: "TrunkId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Busses");

            migrationBuilder.DropTable(
                name: "CommercialVehicles");

            migrationBuilder.DropTable(
                name: "PrivateVehicles");

            migrationBuilder.DropTable(
                name: "Trucks");

            migrationBuilder.DropTable(
                name: "Size");
        }
    }
}
