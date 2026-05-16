using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eVrticSistem.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImePrezime = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Uloga = table.Column<int>(type: "int", nullable: false),
                    StatusNaloga = table.Column<int>(type: "int", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DnevniQRCode",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VrijednostKoda = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DatumVazenja = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VrijemeIsteka = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Aktivan = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DnevniQRCode", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Administrator",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Administrator", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Administrator_AspNetUsers_Id",
                        column: x => x.Id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Odgajatelj",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Odgajatelj", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Odgajatelj_AspNetUsers_Id",
                        column: x => x.Id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Roditelj",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roditelj", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Roditelj_AspNetUsers_Id",
                        column: x => x.Id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Grupa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImeGrupe = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OdgajateljId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grupa", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Grupa_Odgajatelj_OdgajateljId",
                        column: x => x.OdgajateljId,
                        principalTable: "Odgajatelj",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Obavijest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
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
                    table.PrimaryKey("PK_Obavijest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Obavijest_Odgajatelj_OdgajateljId",
                        column: x => x.OdgajateljId,
                        principalTable: "Odgajatelj",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Obavijest_Roditelj_RoditeljId",
                        column: x => x.RoditeljId,
                        principalTable: "Roditelj",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Dijete",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
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
                    table.PrimaryKey("PK_Dijete", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dijete_Grupa_GrupaId",
                        column: x => x.GrupaId,
                        principalTable: "Grupa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Dijete_Roditelj_RoditeljId",
                        column: x => x.RoditeljId,
                        principalTable: "Roditelj",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AlergijaDjeteta",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naziv = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DijeteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlergijaDjeteta", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AlergijaDjeteta_Dijete_DijeteId",
                        column: x => x.DijeteId,
                        principalTable: "Dijete",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BolestDjeteta",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naziv = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DijeteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BolestDjeteta", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BolestDjeteta_Dijete_DijeteId",
                        column: x => x.DijeteId,
                        principalTable: "Dijete",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DnevniIzvjestaj",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
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
                    table.PrimaryKey("PK_DnevniIzvjestaj", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DnevniIzvjestaj_Dijete_DijeteId",
                        column: x => x.DijeteId,
                        principalTable: "Dijete",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EvidencijaDolaskaOdlaska",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
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
                    table.PrimaryKey("PK_EvidencijaDolaskaOdlaska", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EvidencijaDolaskaOdlaska_Dijete_DijeteId",
                        column: x => x.DijeteId,
                        principalTable: "Dijete",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EvidencijaDolaskaOdlaska_DnevniQRCode_DnevniQRCodeId",
                        column: x => x.DnevniQRCodeId,
                        principalTable: "DnevniQRCode",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SazetakAktivnosti",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
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
                    table.PrimaryKey("PK_SazetakAktivnosti", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SazetakAktivnosti_Dijete_DijeteId",
                        column: x => x.DijeteId,
                        principalTable: "Dijete",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlergijaDjeteta_DijeteId",
                table: "AlergijaDjeteta",
                column: "DijeteId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Email",
                table: "AspNetUsers",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

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
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

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
                name: "AspNetRoles");

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
                name: "AspNetUsers");
        }
    }
}
