using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppAntiPlagiat.Migrations
{
    /// <inheritdoc />
    public partial class byteTrans : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Chemin",
                table: "Rapports");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateDepot",
                table: "Rapports",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<byte[]>(
                name: "data",
                table: "Rapports",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateDepot",
                table: "Rapports");

            migrationBuilder.DropColumn(
                name: "data",
                table: "Rapports");

            migrationBuilder.AddColumn<string>(
                name: "Chemin",
                table: "Rapports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
