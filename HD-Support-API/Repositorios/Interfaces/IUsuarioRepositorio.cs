using HD_Support_API.Models;
using Microsoft.AspNetCore.Mvc;
using System.Buffers.Text;
using System.ComponentModel.DataAnnotations;

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

        Task<Usuarios> BuscarPorTokenJWT(string token);
        Task<IActionResult> RedefinirSenha(string token, string novaSenha, string confirmacaoSenha);
        Task <IActionResult> RedefinirEmail(string email, int id);
        Task<IActionResult> ConfirmarEmail(string token, string email);
    }
}
