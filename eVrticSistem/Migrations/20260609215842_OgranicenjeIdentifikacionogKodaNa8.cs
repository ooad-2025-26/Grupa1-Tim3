using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eVrticSistem.Migrations
{
    /// <inheritdoc />
    public partial class OgranicenjeIdentifikacionogKodaNa8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
UPDATE [Dijete]
SET [IdentifikacioniKod] = LTRIM(RTRIM([IdentifikacioniKod]))
WHERE [IdentifikacioniKod] IS NOT NULL;
");

            migrationBuilder.Sql(@"
UPDATE [Dijete]
SET [IdentifikacioniKod] = 'D' + RIGHT('0000000' + CAST([Id] AS varchar(7)), 7)
WHERE [IdentifikacioniKod] IS NULL
   OR LEN([IdentifikacioniKod]) <> 8;
");

            migrationBuilder.AlterColumn<string>(
     name: "IdentifikacioniKod",
     table: "Dijete",
     type: "nvarchar(8)",
     maxLength: 8,
     nullable: false,
     collation: "Latin1_General_100_CS_AS",
     oldClrType: typeof(string),
     oldType: "nvarchar(30)",
     oldMaxLength: 30);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "IdentifikacioniKod",
                table: "Dijete",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(8)",
                oldMaxLength: 8);
        }
    }
}
