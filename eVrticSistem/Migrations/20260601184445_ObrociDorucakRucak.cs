using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eVrticSistem.Migrations
{
    /// <inheritdoc />
    public partial class ObrociDorucakRucak : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Obrok",
                table: "DnevniIzvjestaj");

            migrationBuilder.RenameColumn(
                name: "StatusObroka",
                table: "DnevniIzvjestaj",
                newName: "StatusRucka");

            migrationBuilder.AddColumn<string>(
                name: "Dorucak",
                table: "DnevniIzvjestaj",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Rucak",
                table: "DnevniIzvjestaj",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "StatusDorucka",
                table: "DnevniIzvjestaj",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Dorucak",
                table: "DnevniIzvjestaj");

            migrationBuilder.DropColumn(
                name: "Rucak",
                table: "DnevniIzvjestaj");

            migrationBuilder.DropColumn(
                name: "StatusDorucka",
                table: "DnevniIzvjestaj");

            migrationBuilder.RenameColumn(
                name: "StatusRucka",
                table: "DnevniIzvjestaj",
                newName: "StatusObroka");

            migrationBuilder.AddColumn<string>(
                name: "Obrok",
                table: "DnevniIzvjestaj",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
