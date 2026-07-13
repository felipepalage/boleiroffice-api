using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Boleiroffice.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddJogadoresPorTimeRachao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "JogadoresPorTime",
                table: "rachao_eventos",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JogadoresPorTime",
                table: "rachao_eventos");
        }
    }
}
