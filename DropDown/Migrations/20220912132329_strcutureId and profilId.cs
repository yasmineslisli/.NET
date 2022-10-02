using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DropDown.Migrations
{
    public partial class strcutureIdandprofilId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Profils_profilId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Structures_structureId",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "structureId",
                table: "Users",
                newName: "StructureId");

            migrationBuilder.RenameColumn(
                name: "profilId",
                table: "Users",
                newName: "ProfilId");

            migrationBuilder.RenameIndex(
                name: "IX_Users_structureId",
                table: "Users",
                newName: "IX_Users_StructureId");

            migrationBuilder.RenameIndex(
                name: "IX_Users_profilId",
                table: "Users",
                newName: "IX_Users_ProfilId");

            migrationBuilder.AlterColumn<int>(
                name: "StructureId",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ProfilId",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Profils_ProfilId",
                table: "Users",
                column: "ProfilId",
                principalTable: "Profils",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Structures_StructureId",
                table: "Users",
                column: "StructureId",
                principalTable: "Structures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Profils_ProfilId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Structures_StructureId",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "StructureId",
                table: "Users",
                newName: "structureId");

            migrationBuilder.RenameColumn(
                name: "ProfilId",
                table: "Users",
                newName: "profilId");

            migrationBuilder.RenameIndex(
                name: "IX_Users_StructureId",
                table: "Users",
                newName: "IX_Users_structureId");

            migrationBuilder.RenameIndex(
                name: "IX_Users_ProfilId",
                table: "Users",
                newName: "IX_Users_profilId");

            migrationBuilder.AlterColumn<int>(
                name: "structureId",
                table: "Users",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "profilId",
                table: "Users",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Profils_profilId",
                table: "Users",
                column: "profilId",
                principalTable: "Profils",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Structures_structureId",
                table: "Users",
                column: "structureId",
                principalTable: "Structures",
                principalColumn: "Id");
        }
    }
}
