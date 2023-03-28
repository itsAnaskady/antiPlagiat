using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppAntiPlagiat.Migrations
{
    /// <inheritdoc />
    public partial class ajoutDateModifRapport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateModif",
                table: "Rapports",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateModif",
                table: "Rapports");
        }
    }
}
