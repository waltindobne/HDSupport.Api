using HD_Support_API.Metodos;
using HD_Support_API.Models;
using HD_Support_API.Repositorios.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace HD_Support_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioRepositorio _repositorio;
        private readonly IEmailSender _SendEmailRepository;

        [HttpGet]
        [Route("Anônimo")]
        [AllowAnonymous]
        public string Anonimo() => "Anônimo";

        [HttpGet]
        [Route("Autenticado")]
        [Authorize]
        public string Autenticado() => $"Autenticado - {User.Identity.Name}";
        public UsuarioController(IUsuarioRepositorio repositorio, IEmailSender emailSender)
        {
            _repositorio = repositorio;
            _SendEmailRepository = emailSender;
        }

        [HttpGet]
        [Route("Lista-HelpDesk")]
        [Authorize(Roles = "Gerente,HelpDesk,RH")]
        public async Task<IActionResult> ListarHelpDesk()
        {
            var ListaHelpDesk = await _repositorio.ListarHelpDesk();
            return Ok(ListaHelpDesk);
        }
        [HttpGet]
        [Route("Lista-Funcionarios")]
        [Authorize(Roles = "Gerente,HelpDesk,RH")]
        public async Task<IActionResult> ListarFuncionarios()
        {
            var ListaFuncionarios = await _repositorio.ListarFuncionario();
            return Ok(ListaFuncionarios);
        }

        [HttpPost]
        [Route("Registro")]
        [AllowAnonymous]
        public async Task<IActionResult> AdicionarUsuario([FromBody] Usuarios usuario)
        {
            if (usuario == null)
            {
                return BadRequest("Dados do usuário não fornecidos");
            }

            var usuarioAdicionado = await _repositorio.AdicionarUsuario(usuario);

            return Ok(usuarioAdicionado);
        }

        [HttpPut]
        [Route("Editar-Usuario/{id}")]
        [Authorize(Roles = "Gerente,HelpDesk,RH")]
        public async Task<IActionResult> AtualizarUsuario(int id, [FromBody] Usuarios usuario)
        {
            if (usuario == null)
            {
                return BadRequest($"Cadastro com ID:{id} não encontrado");
            }

            var AtualizarHelpDesk = await _repositorio.AtualizarUsuario(usuario, id);
            return Ok(AtualizarHelpDesk);
        }

        [HttpGet]
        [Route("Buscar-Usuario-Por-ID/{id}")]
        [Authorize(Roles = "Gerente,HelpDesk,RH")]
        public async Task<IActionResult> BuscarUsuarioPorID(int id)
        {
            var buscaUsuario = await _repositorio.BuscarUsuarioPorId(id);

            if (buscaUsuario == null)
            {
                return NotFound($"Cadastro com ID:{id} não encontrado");
            }

            return Ok(buscaUsuario);
        }

        [HttpPost]
        [Route("Excluir-Usuario/{id}")]
        [Authorize(Roles = "Gerente,HelpDesk,RH")]
        public async Task<IActionResult> ExcluirUsuario(int id)
        {
            var ExcluirPerfil = await _repositorio.ExcluirUsuario(id);

            if (ExcluirPerfil != true)
            {
                return NotFound($"Cadastro com ID:{id} não encontrado");
            }

            return Ok(ExcluirPerfil);
        }

        [HttpGet]
        [Route("BuscarPorTokenJWT/{token}")]
        [Authorize(Roles = "Funcionario,Gerente,HelpDesk,RH")]
        public async Task<IActionResult> BuscarPorTokenJWT(string token)
        {
            var Perfil = await _repositorio.BuscarPorTokenJWT(token);

            if (Perfil == null)
            {
                return NotFound($"Perfil não encontrado");
            }

            return Ok(Perfil);
        }

        [HttpPost]
        [Route("Login")]
        [AllowAnonymous]
        public async Task<ActionResult<dynamic>> Login(string email, string senha)
        {
            var result = await _repositorio.Login(email, senha);

            if (result == null)
            {
                return Unauthorized("Não autorizado");
            }
            var token = TokenService.GenerateToken(result);
            return new
            {
                usuario = result,
                token = token
            };
        }

        [HttpPut]
        [Route("Atualizar-Status-Usuario/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> AtualizarStatusHelpDesk(int id, int status)
        {
            var result = await _repositorio.AtualizarStatus(id, status);

            return Ok(result);
        }

        [HttpPost]
        [Route("Recuperacao-Senha")]
        [AllowAnonymous]
        public async Task<IActionResult> EnviarEmail(string email)
        {
            try
            {
                await _repositorio.RecuperarSenha(email);
                return Ok("E-mail de recuperação de senha enviado com sucesso.");
            }
            catch
            {
                return StatusCode(500, $"Ocorreu um erro ao enviar o e-mail de recuperação de senha");
            }
        }
        [HttpPost]
        [Route("RedefinirSenha")]
        [AllowAnonymous]
        public async Task<IActionResult> RedefinirSenha(string token, string novaSenha,string confirmacaoSenha)
        {
            return await _repositorio.RedefinirSenha(token, novaSenha,confirmacaoSenha);
        }

        [HttpPost]
        [Route("ConfirmarEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmarEmail(string token)
        {
            return await _repositorio.ConfirmarEmail(token);
        }
    }
}
