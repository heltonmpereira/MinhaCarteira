using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinhaCarteira.Modelo.Migrations
{
    /// <inheritdoc />
    public partial class SerilogMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Log",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Deletado = table.Column<bool>(type: "boolean", nullable: false),
                    DataHora = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    TipoLog = table.Column<int>(type: "integer", nullable: false),
                    Categoria = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Mensagem = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    DadosSerializados = table.Column<string>(type: "text", nullable: true),
                    StackTrace = table.Column<string>(type: "text", nullable: true),
                    IpUsuario = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    UserAgent = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Url = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    MetodoHttp = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    StatusCode = table.Column<int>(type: "integer", nullable: true),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: true),
                    OrganizacaoId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Log", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Log_Organizacao_OrganizacaoId",
                        column: x => x.OrganizacaoId,
                        principalTable: "Organizacao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Log_Usuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Log_OrganizacaoId",
                table: "Log",
                column: "OrganizacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_Log_UsuarioId",
                table: "Log",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Log");
        }
    }
}
