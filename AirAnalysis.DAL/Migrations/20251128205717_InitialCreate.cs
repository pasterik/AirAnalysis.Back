using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirAnalysis.DAL.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Phenomens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Phenomens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Places",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Places", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RecordData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<double>(type: "float", nullable: false),
                    DateRecord = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PhenomenId = table.Column<int>(type: "int", nullable: true),
                    PlaceId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecordData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecordData_Phenomens_PhenomenId",
                        column: x => x.PhenomenId,
                        principalTable: "Phenomens",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RecordData_Places_PlaceId",
                        column: x => x.PlaceId,
                        principalTable: "Places",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "idx_recorddata_main",
                table: "RecordData",
                columns: new[] { "PlaceId", "DateRecord", "PhenomenId" });

            migrationBuilder.CreateIndex(
                name: "IX_RecordData_PhenomenId",
                table: "RecordData",
                column: "PhenomenId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecordData");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Phenomens");

            migrationBuilder.DropTable(
                name: "Places");
        }
    }
}
