using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgenciadeTours.Migrations
{
    /// <inheritdoc />
    public partial class AgenciadeTours : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Paises",
                columns: table => new
                {
                    PaisID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Paises", x => x.PaisID);
                });

            migrationBuilder.CreateTable(
                name: "Destinos",
                columns: table => new
                {
                    DestinoID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PaisID = table.Column<int>(type: "int", nullable: false),
                    Dias_Duracion = table.Column<int>(type: "int", nullable: false),
                    Horas_Duracion = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Destinos", x => x.DestinoID);
                    table.ForeignKey(
                        name: "FK_Destinos_Paises_PaisID",
                        column: x => x.PaisID,
                        principalTable: "Paises",
                        principalColumn: "PaisID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tours",
                columns: table => new
                {
                    TourID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PaisID = table.Column<int>(type: "int", nullable: false),
                    DestinoID = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Hora = table.Column<TimeSpan>(type: "time", nullable: false),
                    Precio = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tours", x => x.TourID);
                    table.ForeignKey(
                        name: "FK_Tours_Destinos_DestinoID",
                        column: x => x.DestinoID,
                        principalTable: "Destinos",
                        principalColumn: "DestinoID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tours_Paises_PaisID",
                        column: x => x.PaisID,
                        principalTable: "Paises",
                        principalColumn: "PaisID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Destinos_PaisID",
                table: "Destinos",
                column: "PaisID");

            migrationBuilder.CreateIndex(
                name: "IX_Tours_DestinoID",
                table: "Tours",
                column: "DestinoID");

            migrationBuilder.CreateIndex(
                name: "IX_Tours_PaisID",
                table: "Tours",
                column: "PaisID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tours");

            migrationBuilder.DropTable(
                name: "Destinos");

            migrationBuilder.DropTable(
                name: "Paises");
        }
    }
}
