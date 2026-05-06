using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eVrtic.Migrations
{
    /// <inheritdoc />
    public partial class SyncDatabaseWithIdNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Administrator_Korisnik_IdKorisnika",
                table: "Administrator");

            migrationBuilder.DropForeignKey(
                name: "FK_Odgajatelj_Korisnik_IdKorisnika",
                table: "Odgajatelj");

            migrationBuilder.DropForeignKey(
                name: "FK_Roditelj_Korisnik_IdKorisnika",
                table: "Roditelj");

            migrationBuilder.RenameColumn(
                name: "IdSazetka",
                table: "SazetakAktivnosti",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "IdKorisnika",
                table: "Roditelj",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "IdKorisnika",
                table: "Odgajatelj",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "IdObavijesti",
                table: "Obavijest",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "IdKorisnika",
                table: "Korisnik",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "IdGrupe",
                table: "Grupa",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "IdEvidencije",
                table: "EvidencijaDolaskaOdlaska",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "IdQRCode",
                table: "DnevniQRCode",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "IdIzvjestaja",
                table: "DnevniIzvjestaj",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "IdDjeteta",
                table: "Dijete",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "IdBolesti",
                table: "BolestDjeteta",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "IdAlergije",
                table: "AlergijaDjeteta",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "IdKorisnika",
                table: "Administrator",
                newName: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Administrator_Korisnik_Id",
                table: "Administrator",
                column: "Id",
                principalTable: "Korisnik",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Odgajatelj_Korisnik_Id",
                table: "Odgajatelj",
                column: "Id",
                principalTable: "Korisnik",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Roditelj_Korisnik_Id",
                table: "Roditelj",
                column: "Id",
                principalTable: "Korisnik",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Administrator_Korisnik_Id",
                table: "Administrator");

            migrationBuilder.DropForeignKey(
                name: "FK_Odgajatelj_Korisnik_Id",
                table: "Odgajatelj");

            migrationBuilder.DropForeignKey(
                name: "FK_Roditelj_Korisnik_Id",
                table: "Roditelj");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "SazetakAktivnosti",
                newName: "IdSazetka");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Roditelj",
                newName: "IdKorisnika");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Odgajatelj",
                newName: "IdKorisnika");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Obavijest",
                newName: "IdObavijesti");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Korisnik",
                newName: "IdKorisnika");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Grupa",
                newName: "IdGrupe");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "EvidencijaDolaskaOdlaska",
                newName: "IdEvidencije");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "DnevniQRCode",
                newName: "IdQRCode");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "DnevniIzvjestaj",
                newName: "IdIzvjestaja");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Dijete",
                newName: "IdDjeteta");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "BolestDjeteta",
                newName: "IdBolesti");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "AlergijaDjeteta",
                newName: "IdAlergije");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Administrator",
                newName: "IdKorisnika");

            migrationBuilder.AddForeignKey(
                name: "FK_Administrator_Korisnik_IdKorisnika",
                table: "Administrator",
                column: "IdKorisnika",
                principalTable: "Korisnik",
                principalColumn: "IdKorisnika",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Odgajatelj_Korisnik_IdKorisnika",
                table: "Odgajatelj",
                column: "IdKorisnika",
                principalTable: "Korisnik",
                principalColumn: "IdKorisnika",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Roditelj_Korisnik_IdKorisnika",
                table: "Roditelj",
                column: "IdKorisnika",
                principalTable: "Korisnik",
                principalColumn: "IdKorisnika",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
