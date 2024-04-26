using HD_Support_API.Models;
using HD_Support_API.Repositorios.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HD_Support_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EquipamentosController : ControllerBase
    {
        private readonly IEquipamentosRepositorio _repositorio;

        public EquipamentosController(IEquipamentosRepositorio repositorio)
        {
            _repositorio = repositorio;
        }

        [HttpGet]
        [Route("Lista-Equipamentos")]
        [Authorize(Roles = "Gerente,HelpDesk,RH")]
        public async Task<IActionResult> ListarEquipamentos()
        {
            var ListaHelpDesk = await _repositorio.ListarEquipamentos();
            return Ok(ListaHelpDesk);
        }

        [HttpPost]
        [Route("Registro-Equipamentos")]
        [Authorize(Roles = "Gerente,HelpDesk")]
        public async Task<IActionResult> AdicionarEquipamentos([FromBody] Equipamentos equipamentos)
        {
            if (equipamentos == null)
            {
                return BadRequest("Dados da maquina não fornecidos corretamente");
            }

            var equipamentoAdicionado = await _repositorio.AdicionarEquipamento(equipamentos);

            return Ok(equipamentoAdicionado);
        }

        [HttpPut]
        [Route("Editar-Maquina/{id}")]
        [Authorize(Roles = "Gerente,HelpDesk")]
        public async Task<IActionResult> AtualizarMaquina(int id, [FromBody] Equipamentos equipamentos)
        {
            if (equipamentos == null)
            {
                return BadRequest($"Maquina não encontrada");
            }

            var atualizarEquipamento = await _repositorio.AtualizarEquipamento(equipamentos, id);
            return Ok(atualizarEquipamento);
        }

        [HttpGet]
        [Route("Buscar-Equipamento-Por-ID/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> BuscarEquipamentosPorId(int id)
        {
            var busca = await _repositorio.BuscarEquipamentosPorId(id);

            if (busca == null)
            {
                return NotFound($"Cadastro com ID:{id} não encontrado");
            }

            return Ok(busca);
        }

        [HttpPost]
        [Route("Excluir-Maquina/{id}")]
        [Authorize(Roles = "Gerente,HelpDesk")]
        public async Task<IActionResult> ExcluirEquipamento(int id)
        {
            var excluirMaquina = await _repositorio.ExcluirEquipamento(id);

            if (excluirMaquina)
            {
                return Ok(excluirMaquina);
            }

            return BadRequest("Erro ao excluir o equipamento.");
        }

        [HttpGet]
        [Route("Dados-Equipamento-Pizza")]
        [AllowAnonymous]
        public async Task<IActionResult> DadosEquipamentoPizza()
        {
            var dados = await _repositorio.DadosEquipamentoPizza();

            if (dados != null)
            {
                return Ok(dados);
            }

            return BadRequest("Erro ao procurar os dados no banco de dados");
        }
    }
}

