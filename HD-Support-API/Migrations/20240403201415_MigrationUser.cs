using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HD_Support_API.Migrations
{
    /// <inheritdoc />
    public partial class MigrationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Conversa_HelpDesk_ClienteId",
                table: "Conversa");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversa_HelpDesk_FuncionarioId",
                table: "Conversa");

            migrationBuilder.DropForeignKey(
                name: "FK_Emprestimo_Funcionario_FuncionariosId",
                table: "Emprestimo");

            migrationBuilder.DropForeignKey(
                name: "FK_Mensagens_HelpDesk_UsuarioId",
                table: "Mensagens");

            migrationBuilder.DropTable(
                name: "Funcionario");

            migrationBuilder.DropTable(
                name: "HelpDesk");

            migrationBuilder.DropIndex(
                name: "IX_Emprestimo_FuncionariosId",
                table: "Emprestimo");

            migrationBuilder.AddColumn<int>(
                name: "FuncionarioId",
                table: "Emprestimo",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Senha = table.Column<string>(type: "TEXT", nullable: false),
                    Telefone = table.Column<string>(type: "TEXT", nullable: false),
                    Cargo = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: true),
                    StatusConversa = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Emprestimo_FuncionarioId",
                table: "Emprestimo",
                column: "FuncionarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Conversa_Usuarios_ClienteId",
                table: "Conversa",
                column: "ClienteId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Conversa_Usuarios_FuncionarioId",
                table: "Conversa",
                column: "FuncionarioId",
                principalTable: "Usuarios",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Emprestimo_Usuarios_FuncionarioId",
                table: "Emprestimo",
                column: "FuncionarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Mensagens_Usuarios_UsuarioId",
                table: "Mensagens",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Conversa_Usuarios_ClienteId",
                table: "Conversa");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversa_Usuarios_FuncionarioId",
                table: "Conversa");

            migrationBuilder.DropForeignKey(
                name: "FK_Emprestimo_Usuarios_FuncionarioId",
                table: "Emprestimo");

            migrationBuilder.DropForeignKey(
                name: "FK_Mensagens_Usuarios_UsuarioId",
                table: "Mensagens");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Emprestimo_FuncionarioId",
                table: "Emprestimo");

            migrationBuilder.DropColumn(
                name: "FuncionarioId",
                table: "Emprestimo");

            migrationBuilder.CreateTable(
                name: "Funcionario",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Categoria = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Nome = table.Column<string>(type: "TEXT", nullable: false),
                    StatusFuncionario = table.Column<string>(type: "TEXT", nullable: false),
                    Telefone = table.Column<string>(type: "TEXT", nullable: false),
                    Telegram = table.Column<string>(type: "TEXT", nullable: false),
                    profissional_HD = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Funcionario", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HelpDesk",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Nome = table.Column<string>(type: "TEXT", nullable: false),
                    Senha = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: true),
                    StatusConversa = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HelpDesk", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Emprestimo_FuncionariosId",
                table: "Emprestimo",
                column: "FuncionariosId");

            migrationBuilder.AddForeignKey(
                name: "FK_Conversa_HelpDesk_ClienteId",
                table: "Conversa",
                column: "ClienteId",
                principalTable: "HelpDesk",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Conversa_HelpDesk_FuncionarioId",
                table: "Conversa",
                column: "FuncionarioId",
                principalTable: "HelpDesk",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Emprestimo_Funcionario_FuncionariosId",
                table: "Emprestimo",
                column: "FuncionariosId",
                principalTable: "Funcionario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Mensagens_HelpDesk_UsuarioId",
                table: "Mensagens",
                column: "UsuarioId",
                principalTable: "HelpDesk",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
