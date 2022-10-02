using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DropDown.Migrations
{
    public partial class AddObjectifwithdrandexerciceandaction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DRs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DRs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Exercices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Annee = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exercices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Objectifs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActionProjId = table.Column<int>(type: "int", nullable: false),
                    ExerciceId = table.Column<int>(type: "int", nullable: false),
                    DRId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Objectifs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Objectifs_ActionProjs_ActionProjId",
                        column: x => x.ActionProjId,
                        principalTable: "ActionProjs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Objectifs_DRs_DRId",
                        column: x => x.DRId,
                        principalTable: "DRs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Objectifs_Exercices_ExerciceId",
                        column: x => x.ExerciceId,
                        principalTable: "Exercices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Objectifs_ActionProjId",
                table: "Objectifs",
                column: "ActionProjId");

            migrationBuilder.CreateIndex(
                name: "IX_Objectifs_DRId",
                table: "Objectifs",
                column: "DRId");

            migrationBuilder.CreateIndex(
                name: "IX_Objectifs_ExerciceId",
                table: "Objectifs",
                column: "ExerciceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Objectifs");

            migrationBuilder.DropTable(
                name: "DRs");

            migrationBuilder.DropTable(
                name: "Exercices");
        }
    }
}
