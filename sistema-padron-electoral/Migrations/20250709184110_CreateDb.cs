using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sistema_padron_electoral.Migrations
{
    /// <inheritdoc />
    public partial class CreateDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Votante",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CI = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NombreCompleto = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FotoCarnetAnverso = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FotoCarnetReverso = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FotoVotante = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    RecintoId = table.Column<int>(type: "int", nullable: false),
                    RecintoNombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Votante", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Votante");
        }
    }
}
