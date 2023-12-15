using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniProjet_.NET.Migrations
{
    /// <inheritdoc />
    public partial class fix2222 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RevendeurCommandeId",
                table: "Variations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Variations_RevendeurCommandeId",
                table: "Variations",
                column: "RevendeurCommandeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Variations_RevendeurCommandes_RevendeurCommandeId",
                table: "Variations",
                column: "RevendeurCommandeId",
                principalTable: "RevendeurCommandes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Variations_RevendeurCommandes_RevendeurCommandeId",
                table: "Variations");

            migrationBuilder.DropIndex(
                name: "IX_Variations_RevendeurCommandeId",
                table: "Variations");

            migrationBuilder.DropColumn(
                name: "RevendeurCommandeId",
                table: "Variations");
        }
    }
}
