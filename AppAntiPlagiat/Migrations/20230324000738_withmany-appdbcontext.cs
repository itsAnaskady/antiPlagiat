using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppAntiPlagiat.Migrations
{
    /// <inheritdoc />
    public partial class withmanyappdbcontext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rapports_AspNetUsers_EtudiantId",
                table: "Rapports");

            migrationBuilder.AddColumn<string>(
                name: "UtilisateurId",
                table: "Rapports",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rapports_UtilisateurId",
                table: "Rapports",
                column: "UtilisateurId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rapports_AspNetUsers_EtudiantId",
                table: "Rapports",
                column: "EtudiantId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Rapports_AspNetUsers_UtilisateurId",
                table: "Rapports",
                column: "UtilisateurId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rapports_AspNetUsers_EtudiantId",
                table: "Rapports");

            migrationBuilder.DropForeignKey(
                name: "FK_Rapports_AspNetUsers_UtilisateurId",
                table: "Rapports");

            migrationBuilder.DropIndex(
                name: "IX_Rapports_UtilisateurId",
                table: "Rapports");

            migrationBuilder.DropColumn(
                name: "UtilisateurId",
                table: "Rapports");

            migrationBuilder.AddForeignKey(
                name: "FK_Rapports_AspNetUsers_EtudiantId",
                table: "Rapports",
                column: "EtudiantId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
