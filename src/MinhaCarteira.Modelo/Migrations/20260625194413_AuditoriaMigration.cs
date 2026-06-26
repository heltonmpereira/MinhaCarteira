using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinhaCarteira.Modelo.Migrations
{
    /// <inheritdoc />
    public partial class AuditoriaMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Auditoria",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Deletado = table.Column<bool>(type: "boolean", nullable: false),
                    DataHora = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Acao = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Entidade = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    EntidadeId = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    DadosAntigos = table.Column<string>(type: "text", nullable: true),
                    DadosNovos = table.Column<string>(type: "text", nullable: true),
                    IpUsuario = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    UserAgent = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Rotina = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: true),
                    OrganizacaoId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auditoria", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Auditoria_Organizacao_OrganizacaoId",
                        column: x => x.OrganizacaoId,
                        principalTable: "Organizacao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Auditoria_Usuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Auditoria_OrganizacaoId",
                table: "Auditoria",
                column: "OrganizacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_Auditoria_UsuarioId",
                table: "Auditoria",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Auditoria");
        }
    }
}
