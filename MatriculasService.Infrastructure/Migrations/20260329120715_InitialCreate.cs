using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MatriculasService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "matriculas",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    aluno_id = table.Column<Guid>(type: "uuid", nullable: false),
                    disciplina_id = table.Column<Guid>(type: "uuid", nullable: false),
                    periodo_ano = table.Column<int>(type: "integer", nullable: false),
                    periodo_semestre = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_matriculas", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_matriculas_aluno_disciplina_periodo",
                table: "matriculas",
                columns: new[] { "aluno_id", "disciplina_id", "periodo_ano", "periodo_semestre" },
                filter: "status = 'Confirmada'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "matriculas");
        }
    }
}
