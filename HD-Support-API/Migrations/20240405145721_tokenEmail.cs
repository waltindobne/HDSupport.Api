using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HD_Support_API.Migrations
{
    /// <inheritdoc />
    public partial class tokenEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Emprestimo_Usuarios_FuncionarioId",
                table: "Emprestimo");

            migrationBuilder.DropIndex(
                name: "IX_Emprestimo_FuncionarioId",
                table: "Emprestimo");

            migrationBuilder.DropColumn(
                name: "FuncionarioId",
                table: "Emprestimo");

            migrationBuilder.DropColumn(
                name: "profissional_hd",
                table: "Emprestimo");

            migrationBuilder.RenameColumn(
                name: "funcionariosid",
                table: "Emprestimo",
                newName: "usuarioId");

            migrationBuilder.AddColumn<string>(
                name: "DataHoraGeracaoToken",
                table: "Usuarios",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TokenRedefinicaoSenha",
                table: "Usuarios",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Emprestimo_usuarioId",
                table: "Emprestimo",
                column: "usuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Emprestimo_Usuarios_usuarioId",
                table: "Emprestimo",
                column: "usuarioId",
                principalTable: "Usuarios",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Emprestimo_Usuarios_usuarioId",
                table: "Emprestimo");

            migrationBuilder.DropIndex(
                name: "IX_Emprestimo_usuarioId",
                table: "Emprestimo");

            migrationBuilder.DropColumn(
                name: "DataHoraGeracaoToken",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "TokenRedefinicaoSenha",
                table: "Usuarios");

            migrationBuilder.RenameColumn(
                name: "usuarioId",
                table: "Emprestimo",
                newName: "funcionariosid");

            migrationBuilder.AddColumn<int>(
                name: "FuncionarioId",
                table: "Emprestimo",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "profissional_hd",
                table: "Emprestimo",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Emprestimo_FuncionarioId",
                table: "Emprestimo",
                column: "FuncionarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Emprestimo_Usuarios_FuncionarioId",
                table: "Emprestimo",
                column: "FuncionarioId",
                principalTable: "Usuarios",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
