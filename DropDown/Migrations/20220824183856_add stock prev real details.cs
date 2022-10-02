using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DropDown.Migrations
{
    public partial class addstockprevrealdetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DDs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DDs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Prévisions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nombre = table.Column<int>(type: "int", nullable: false),
                    Superficie = table.Column<int>(type: "int", nullable: false),
                    Valeur = table.Column<int>(type: "int", nullable: false),
                    date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    objectifId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prévisions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Prévisions_Objectifs_objectifId",
                        column: x => x.objectifId,
                        principalTable: "Objectifs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Réalisations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nombre = table.Column<int>(type: "int", nullable: false),
                    Superficie = table.Column<int>(type: "int", nullable: false),
                    Valeur = table.Column<int>(type: "int", nullable: false),
                    date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    objectifId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Réalisations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Réalisations_Objectifs_objectifId",
                        column: x => x.objectifId,
                        principalTable: "Objectifs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Stocks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nombre = table.Column<int>(type: "int", nullable: false),
                    Superficie = table.Column<int>(type: "int", nullable: false),
                    Valeur = table.Column<int>(type: "int", nullable: false),
                    date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    objectifId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stocks_Objectifs_objectifId",
                        column: x => x.objectifId,
                        principalTable: "Objectifs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Details",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    numDossier = table.Column<int>(type: "int", nullable: false),
                    TRN = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    indiceTRN = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    numTRN = table.Column<int>(type: "int", nullable: false),
                    Superficie = table.Column<int>(type: "int", nullable: false),
                    Valeur = table.Column<int>(type: "int", nullable: false),
                    DDId = table.Column<int>(type: "int", nullable: false),
                    PrévisionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Details", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Details_DDs_DDId",
                        column: x => x.DDId,
                        principalTable: "DDs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Details_Prévisions_PrévisionId",
                        column: x => x.PrévisionId,
                        principalTable: "Prévisions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Details_DDId",
                table: "Details",
                column: "DDId");

            migrationBuilder.CreateIndex(
                name: "IX_Details_PrévisionId",
                table: "Details",
                column: "PrévisionId");

            migrationBuilder.CreateIndex(
                name: "IX_Prévisions_objectifId",
                table: "Prévisions",
                column: "objectifId");

            migrationBuilder.CreateIndex(
                name: "IX_Réalisations_objectifId",
                table: "Réalisations",
                column: "objectifId");

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_objectifId",
                table: "Stocks",
                column: "objectifId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Details");

            migrationBuilder.DropTable(
                name: "Réalisations");

            migrationBuilder.DropTable(
                name: "Stocks");

            migrationBuilder.DropTable(
                name: "DDs");

            migrationBuilder.DropTable(
                name: "Prévisions");
        }
    }
}
