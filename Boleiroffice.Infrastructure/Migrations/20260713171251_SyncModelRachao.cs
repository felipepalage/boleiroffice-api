using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Boleiroffice.Infrastructure.Migrations
{
    /// <summary>
    /// No-op: existe só para sincronizar o snapshot do modelo. A coluna
    /// JogadoresPorTime já é criada pela migration AddJogadoresPorTimeRachao;
    /// sem esta sincronização o EF Core 9 dispara PendingModelChangesWarning
    /// e derruba o startup da API.
    /// </summary>
    public partial class SyncModelRachao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
