using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppAntiPlagiat.Migrations
{
    /// <inheritdoc />
    public partial class withmanyappdbcontext2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UtilisateurId",
                table: "Encadre",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Encadre_UtilisateurId",
                table: "Encadre",
                column: "UtilisateurId");

            migrationBuilder.AddForeignKey(
                name: "FK_Encadre_AspNetUsers_UtilisateurId",
                table: "Encadre",
                column: "UtilisateurId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Encadre_AspNetUsers_UtilisateurId",
                table: "Encadre");

            migrationBuilder.DropIndex(
                name: "IX_Encadre_UtilisateurId",
                table: "Encadre");

            migrationBuilder.DropColumn(
                name: "UtilisateurId",
                table: "Encadre");
        }
    }
}
