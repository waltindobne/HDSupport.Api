using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HD_Support_API.Migrations
{
    /// <inheritdoc />
    public partial class tokenEmail2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Emprestimo_Usuarios_usuarioId",
                table: "Emprestimo");

            migrationBuilder.RenameColumn(
                name: "usuarioId",
                table: "Emprestimo",
                newName: "usuarioid");

            migrationBuilder.RenameIndex(
                name: "IX_Emprestimo_usuarioId",
                table: "Emprestimo",
                newName: "IX_Emprestimo_UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Emprestimo_Usuarios_UsuarioId",
                table: "Emprestimo",
                column: "usuarioid",
                principalTable: "Usuarios",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Emprestimo_Usuarios_UsuarioId",
                table: "Emprestimo");

            migrationBuilder.RenameColumn(
                name: "usuarioid",
                table: "Emprestimo",
                newName: "usuarioId");

            migrationBuilder.RenameIndex(
                name: "IX_Emprestimo_UsuarioId",
                table: "Emprestimo",
                newName: "IX_Emprestimo_usuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Emprestimo_Usuarios_usuarioId",
                table: "Emprestimo",
                column: "usuarioId",
                principalTable: "Usuarios",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
