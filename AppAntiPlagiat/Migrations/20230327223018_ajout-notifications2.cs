using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppAntiPlagiat.Migrations
{
    /// <inheritdoc />
    public partial class ajoutnotifications2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateNotif",
                table: "Notifications",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserIdDesti",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "Vu",
                table: "Notifications",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateNotif",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "UserIdDesti",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Vu",
                table: "Notifications");
        }
    }
}
