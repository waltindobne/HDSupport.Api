using HD_Support_API.Models;
using HD_Support_API.Repositorios.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HD_Support_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ConversaController : ControllerBase
    {
        private readonly IConversaRepositorio _repositorio;

        public ConversaController(IConversaRepositorio repositorio)
        {
            _repositorio = repositorio;
        }

        [HttpGet]
        [Route("Lista-Mensagens")]
        [Authorize(Roles = "Gerente,HelpDesk,RH,Funcionario")]
        public async Task<IActionResult> ListarMensagens(int idConversa)
        {
            var listaMensagens = await _repositorio.ListarMensagens(idConversa);
            return Ok(listaMensagens);
        }

        [HttpPost]
        [Route("Registro-Conversa")]
        [Authorize(Roles = "Gerente,HelpDesk,RH,Funcionario")]
        public async Task<IActionResult> IniciarConversa([FromBody] Conversa conversa)
        {
            if (conversa == null)
            {
                return BadRequest("Dados da conversa não fornecidos");
            }

            var conversaAdicionada = await _repositorio.IniciarConversa(conversa);

            return Ok(conversaAdicionada);
        }

        [HttpPost]
        [Route("Registro-mensagem")]
        [Authorize(Roles = "Gerente,HelpDesk,RH,Funcionario")]
        public async Task<IActionResult> EnviarMensagem(int idConversa, [FromBody] Mensagens mensagem)
        {
            if (mensagem == null)
            {
                return BadRequest("Dados da mensagem não fornecidos");
            }

            var mensagemAdicionada = await _repositorio.EnviarMensagem(idConversa, mensagem);

            return Ok(mensagemAdicionada);
        }

        [HttpGet]
        [Route("Buscar-Conversa-Por-id/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> BuscarConversaPorId(int id)
        {
            var busca = await _repositorio.BuscarConversaPorId(id);

            if (busca == null)
            {
                return NotFound($"Conversa com ID:{id} não encontrado");
            }

            return Ok(busca);
        }

        [HttpPut]
        [Route("Terminar-Conversa/{id}")]
        [Authorize(Roles = "Gerente,HelpDesk")]
        public async Task<IActionResult> TerminarConversa(int id)
        {
            var Terminar = await _repositorio.BuscarConversaPorId(id);
            if (Terminar == null)
            {
                return BadRequest($"Cadastro com ID:{id} não encontrado");
            }

            var atualizarConversa = await _repositorio.TerminarConversa(id);
            return Ok(atualizarConversa);
        }

        [HttpPost]
        [Route("Excluir-mensagem/{id}")]
        [Authorize(Roles = "Gerente,HelpDesk,RH,Funcionario")]
        public async Task<IActionResult> ExcluirMensagem(int id)
        {
            var Excluir = await _repositorio.ExcluirMensagem(id);

            if (Excluir == false)
            {
                return NotFound($"mensagem com ID:{id} não encontrado");
            }

            return Ok(Excluir);
        }

        [HttpGet]
        [Route("Verificar-mensagem-Nova/{idConversa}")]
        [AllowAnonymous]
        public async Task<IActionResult> VerificarMensagemNova(int idConversa, int qtdMensagensAtual)
        {
            var TemMensagemNova = await _repositorio.VerificarMensagemNova(idConversa, qtdMensagensAtual);
            return Ok(TemMensagemNova);
        }

        [HttpPost]
        [Route("Aceitar-Chamado/{idConversa}-{idFuncionario}")]
        [Authorize(Roles = "Gerente,HelpDesk")]
        public async Task<IActionResult> AceitarChamado(int idConversa, int idFuncionario)
        {
            var ChamadoAceito = await _repositorio.AceitarChamado(idConversa, idFuncionario);
            return Ok(ChamadoAceito);
        }

        [HttpGet]
        [Route("Listar-Chamados")]
        [Authorize(Roles = "Gerente,HelpDesk")]
        public async Task<IActionResult> ListarChamados(int tipo, bool aceito = false)
        {
            var Chamados = await _repositorio.ListarChamados(tipo, aceito);
            return Ok(Chamados);
        }

        [HttpGet]
        [Route("Listar-Conversas/{idUsuario}")]
        [Authorize(Roles = "Gerente,HelpDesk,RH,Funcionario")]
        public async Task<IActionResult> ListarConversas(int idUsuario)
        {
            var Conversas = await _repositorio.ListarConversas(idUsuario);
            return Ok(Conversas);
        }

        [HttpPut]
        [Route("Atualizar-status-Conversa/{idConversa}")]
        public async Task<IActionResult> AtualizarStatusConversa(int idConversa, int status)
        {
            var atualizado = await _repositorio.AtualizarStatusConversa(idConversa, status);
            return Ok(atualizado);
        }

        [HttpGet]
        [Route("Dados-Chamados-Dashboard")]
        [Authorize(Roles = "Gerente,HelpDesk,RH")]
        public async Task<IActionResult> DadosChamadosDashboard()
        {
            var dados = await _repositorio.DadosChamadosDashboard();

            if (dados != null)
            {
                return Ok(dados);
            }

            return BadRequest("Erro ao procurar os dados no banco de dados");
        }
    }
}

