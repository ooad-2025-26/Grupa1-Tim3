using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eVrtic.Migrations
{
    /// <inheritdoc />
    public partial class InitialEVrticModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DnevniQRCode",
                columns: table => new
                {
                    IdQRCode = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VrijednostKoda = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DatumVazenja = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VrijemeIsteka = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Aktivan = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DnevniQRCode", x => x.IdQRCode);
                });

            migrationBuilder.CreateTable(
                name: "Korisnik",
                columns: table => new
                {
                    IdKorisnika = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImePrezime = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    LozinkaHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Uloga = table.Column<int>(type: "int", nullable: false),
                    StatusNaloga = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Korisnik", x => x.IdKorisnika);
                });

            migrationBuilder.CreateTable(
                name: "Administrator",
                columns: table => new
                {
                    IdKorisnika = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Administrator", x => x.IdKorisnika);
                    table.ForeignKey(
                        name: "FK_Administrator_Korisnik_IdKorisnika",
                        column: x => x.IdKorisnika,
                        principalTable: "Korisnik",
                        principalColumn: "IdKorisnika",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Odgajatelj",
                columns: table => new
                {
                    IdKorisnika = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Odgajatelj", x => x.IdKorisnika);
                    table.ForeignKey(
                        name: "FK_Odgajatelj_Korisnik_IdKorisnika",
                        column: x => x.IdKorisnika,
                        principalTable: "Korisnik",
                        principalColumn: "IdKorisnika",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Roditelj",
                columns: table => new
                {
                    IdKorisnika = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roditelj", x => x.IdKorisnika);
                    table.ForeignKey(
                        name: "FK_Roditelj_Korisnik_IdKorisnika",
                        column: x => x.IdKorisnika,
                        principalTable: "Korisnik",
                        principalColumn: "IdKorisnika",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Grupa",
                columns: table => new
                {
                    IdGrupe = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImeGrupe = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OdgajateljId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grupa", x => x.IdGrupe);
                    table.ForeignKey(
                        name: "FK_Grupa_Odgajatelj_OdgajateljId",
                        column: x => x.OdgajateljId,
                        principalTable: "Odgajatelj",
                        principalColumn: "IdKorisnika",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Obavijest",
                columns: table => new
                {
                    IdObavijesti = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naslov = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Sadrzaj = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    DatumKreiranja = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DatumSlanja = table.Column<DateTime>(type: "datetime2", nullable: true),
                    KanalSlanja = table.Column<int>(type: "int", nullable: false),
                    StatusObavijesti = table.Column<int>(type: "int", nullable: false),
                    RoditeljId = table.Column<int>(type: "int", nullable: false),
                    OdgajateljId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Obavijest", x => x.IdObavijesti);
                    table.ForeignKey(
                        name: "FK_Obavijest_Odgajatelj_OdgajateljId",
                        column: x => x.OdgajateljId,
                        principalTable: "Odgajatelj",
                        principalColumn: "IdKorisnika");
                    table.ForeignKey(
                        name: "FK_Obavijest_Roditelj_RoditeljId",
                        column: x => x.RoditeljId,
                        principalTable: "Roditelj",
                        principalColumn: "IdKorisnika",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Dijete",
                columns: table => new
                {
                    IdDjeteta = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImePrezime = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IdentifikacioniKod = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    DodatnaNapomena = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Aktivno = table.Column<bool>(type: "bit", nullable: false),
                    GrupaId = table.Column<int>(type: "int", nullable: true),
                    RoditeljId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dijete", x => x.IdDjeteta);
                    table.ForeignKey(
                        name: "FK_Dijete_Grupa_GrupaId",
                        column: x => x.GrupaId,
                        principalTable: "Grupa",
                        principalColumn: "IdGrupe",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Dijete_Roditelj_RoditeljId",
                        column: x => x.RoditeljId,
                        principalTable: "Roditelj",
                        principalColumn: "IdKorisnika",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AlergijaDjeteta",
                columns: table => new
                {
                    IdAlergije = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naziv = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DijeteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlergijaDjeteta", x => x.IdAlergije);
                    table.ForeignKey(
                        name: "FK_AlergijaDjeteta_Dijete_DijeteId",
                        column: x => x.DijeteId,
                        principalTable: "Dijete",
                        principalColumn: "IdDjeteta",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BolestDjeteta",
                columns: table => new
                {
                    IdBolesti = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naziv = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DijeteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BolestDjeteta", x => x.IdBolesti);
                    table.ForeignKey(
                        name: "FK_BolestDjeteta_Dijete_DijeteId",
                        column: x => x.DijeteId,
                        principalTable: "Dijete",
                        principalColumn: "IdDjeteta",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DnevniIzvjestaj",
                columns: table => new
                {
                    IdIzvjestaja = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Datum = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Obrok = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StatusObroka = table.Column<int>(type: "int", nullable: false),
                    SpavanjeMinuta = table.Column<int>(type: "int", nullable: false),
                    VrijemeDolaska = table.Column<TimeSpan>(type: "time", nullable: true),
                    VrijemeOdlaska = table.Column<TimeSpan>(type: "time", nullable: true),
                    NapomenaAktivnosti = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    DatumKreiranja = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DijeteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DnevniIzvjestaj", x => x.IdIzvjestaja);
                    table.ForeignKey(
                        name: "FK_DnevniIzvjestaj_Dijete_DijeteId",
                        column: x => x.DijeteId,
                        principalTable: "Dijete",
                        principalColumn: "IdDjeteta",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EvidencijaDolaskaOdlaska",
                columns: table => new
                {
                    IdEvidencije = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VrijemeDogadjaja = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TipDogadjaja = table.Column<int>(type: "int", nullable: false),
                    UneseniKodDjeteta = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ValidanQRKod = table.Column<bool>(type: "bit", nullable: false),
                    KodDjetetaIspravan = table.Column<bool>(type: "bit", nullable: false),
                    StatusEvidencije = table.Column<int>(type: "int", nullable: false),
                    DijeteId = table.Column<int>(type: "int", nullable: false),
                    DnevniQRCodeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvidencijaDolaskaOdlaska", x => x.IdEvidencije);
                    table.ForeignKey(
                        name: "FK_EvidencijaDolaskaOdlaska_Dijete_DijeteId",
                        column: x => x.DijeteId,
                        principalTable: "Dijete",
                        principalColumn: "IdDjeteta",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EvidencijaDolaskaOdlaska_DnevniQRCode_DnevniQRCodeId",
                        column: x => x.DnevniQRCodeId,
                        principalTable: "DnevniQRCode",
                        principalColumn: "IdQRCode");
                });

            migrationBuilder.CreateTable(
                name: "SazetakAktivnosti",
                columns: table => new
                {
                    IdSazetka = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DatumPocetka = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DatumKraja = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TipSazetka = table.Column<int>(type: "int", nullable: false),
                    BrojObroka = table.Column<int>(type: "int", nullable: false),
                    BrojDolazaka = table.Column<int>(type: "int", nullable: false),
                    AgregiranoSpavanjeMinuta = table.Column<int>(type: "int", nullable: false),
                    OsnovneNapomene = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    DatumGenerisanja = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DijeteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SazetakAktivnosti", x => x.IdSazetka);
                    table.ForeignKey(
                        name: "FK_SazetakAktivnosti_Dijete_DijeteId",
                        column: x => x.DijeteId,
                        principalTable: "Dijete",
                        principalColumn: "IdDjeteta",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlergijaDjeteta_DijeteId",
                table: "AlergijaDjeteta",
                column: "DijeteId");

            migrationBuilder.CreateIndex(
                name: "IX_BolestDjeteta_DijeteId",
                table: "BolestDjeteta",
                column: "DijeteId");

            migrationBuilder.CreateIndex(
                name: "IX_Dijete_GrupaId",
                table: "Dijete",
                column: "GrupaId");

            migrationBuilder.CreateIndex(
                name: "IX_Dijete_IdentifikacioniKod",
                table: "Dijete",
                column: "IdentifikacioniKod",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dijete_RoditeljId",
                table: "Dijete",
                column: "RoditeljId");

            migrationBuilder.CreateIndex(
                name: "IX_DnevniIzvjestaj_DijeteId",
                table: "DnevniIzvjestaj",
                column: "DijeteId");

            migrationBuilder.CreateIndex(
                name: "IX_EvidencijaDolaskaOdlaska_DijeteId",
                table: "EvidencijaDolaskaOdlaska",
                column: "DijeteId");

            migrationBuilder.CreateIndex(
                name: "IX_EvidencijaDolaskaOdlaska_DnevniQRCodeId",
                table: "EvidencijaDolaskaOdlaska",
                column: "DnevniQRCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_Grupa_OdgajateljId",
                table: "Grupa",
                column: "OdgajateljId");

            migrationBuilder.CreateIndex(
                name: "IX_Korisnik_Email",
                table: "Korisnik",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Obavijest_OdgajateljId",
                table: "Obavijest",
                column: "OdgajateljId");

            migrationBuilder.CreateIndex(
                name: "IX_Obavijest_RoditeljId",
                table: "Obavijest",
                column: "RoditeljId");

            migrationBuilder.CreateIndex(
                name: "IX_SazetakAktivnosti_DijeteId",
                table: "SazetakAktivnosti",
                column: "DijeteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Administrator");

            migrationBuilder.DropTable(
                name: "AlergijaDjeteta");

            migrationBuilder.DropTable(
                name: "BolestDjeteta");

            migrationBuilder.DropTable(
                name: "DnevniIzvjestaj");

            migrationBuilder.DropTable(
                name: "EvidencijaDolaskaOdlaska");

            migrationBuilder.DropTable(
                name: "Obavijest");

            migrationBuilder.DropTable(
                name: "SazetakAktivnosti");

            migrationBuilder.DropTable(
                name: "DnevniQRCode");

            migrationBuilder.DropTable(
                name: "Dijete");

            migrationBuilder.DropTable(
                name: "Grupa");

            migrationBuilder.DropTable(
                name: "Roditelj");

            migrationBuilder.DropTable(
                name: "Odgajatelj");

            migrationBuilder.DropTable(
                name: "Korisnik");
        }
    }
}
