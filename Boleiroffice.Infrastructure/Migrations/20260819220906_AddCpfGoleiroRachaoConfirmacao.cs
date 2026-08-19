using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Boleiroffice.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCpfGoleiroRachaoConfirmacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Cpf",
                table: "rachao_confirmacoes",
                type: "character varying(11)",
                maxLength: 11,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "Goleiro",
                table: "rachao_confirmacoes",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cpf",
                table: "rachao_confirmacoes");

            migrationBuilder.DropColumn(
                name: "Goleiro",
                table: "rachao_confirmacoes");
        }
    }
}
