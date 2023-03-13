using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppAntiPlagiat.Migrations
{
    /// <inheritdoc />
    public partial class imgData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IMGurl",
                table: "AspNetUsers",
                newName: "imgType");

            migrationBuilder.AddColumn<byte[]>(
                name: "imgData",
                table: "AspNetUsers",
                type: "varbinary(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "imgData",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "imgType",
                table: "AspNetUsers",
                newName: "IMGurl");
        }
    }
}
