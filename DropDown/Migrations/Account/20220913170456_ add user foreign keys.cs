using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DropDown.Migrations.Account
{
    public partial class adduserforeignkeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddPrimaryKey(
                name: "PK_Structures",
                table: "Structures",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Profils",
                table: "Profils",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Users_ProfilId",
                table: "Users",
                column: "ProfilId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_StructureId",
                table: "Users",
                column: "StructureId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Profils_ProfilId",
                table: "Users",
                column: "ProfilId",
                principalTable: "Profils",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Structures_StructureId",
                table: "Users",
                column: "StructureId",
                principalTable: "Structures",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Profils_ProfilId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Structures_StructureId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_ProfilId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_StructureId",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Structures",
                table: "Structures");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Profils",
                table: "Profils");
        }
    }
}
