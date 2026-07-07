using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Boleiroffice.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddConviteMembro : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "convites_membro",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmpresaId = table.Column<Guid>(type: "uuid", nullable: false),
                    Token = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiraEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_convites_membro", x => x.Id);
                    table.ForeignKey(
                        name: "FK_convites_membro_empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConviteMembro_EmpresaId",
                table: "convites_membro",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_ConviteMembro_Token",
                table: "convites_membro",
                column: "Token",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "convites_membro");
        }
    }
}
