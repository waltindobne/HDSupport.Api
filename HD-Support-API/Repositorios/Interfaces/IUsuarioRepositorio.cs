using HD_Support_API.Models;
using Microsoft.AspNetCore.Mvc;

namespace HD_Support_API.Repositorios.Interfaces
{
    public interface IUsuarioRepositorio
    {
        Task<List<Usuarios>> ListarHelpDesk();
        Task<List<Usuarios>> ListarFuncionario();

        Task<Usuarios> BuscarUsuario(string nome, string telefone);
        Task<Usuarios> BuscarUsuarioPorId(int id);
        Task<int> BuscarPorEmail(string email);

        Task<Usuarios> AdicionarUsuario(Usuarios usuario);
        Task<Usuarios> AtualizarUsuario(Usuarios usuario, int id);
        Task<bool> ExcluirUsuario(int id);
        Task<bool> AtualizarStatus(int id, int status);

        Task<Usuarios> Login(string email, string senha);
        Task RecuperarSenha(string email);


        Task<IActionResult> RedefinirSenha(string token, string novaSenha, string confirmacaoSenha);
        Task<IActionResult> ConfirmarEmail(string token);
    }
}
