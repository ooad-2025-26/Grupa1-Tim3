using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eVrticSistem.Migrations
{
    /// <inheritdoc />
    public partial class DodajTelefonIDatumRodjenja : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "KontaktTelefon",
                table: "Roditelj",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DatumRodjenja",
                table: "Dijete",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KontaktTelefon",
                table: "Roditelj");

            migrationBuilder.DropColumn(
                name: "DatumRodjenja",
                table: "Dijete");
        }
    }
}
