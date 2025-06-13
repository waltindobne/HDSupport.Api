using HD_Support_API.Models;
using HD_Support_API.Repositorios.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HD_Support_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SquadController : ControllerBase
    {
        private readonly ISquadRepositorio _squadRepository;
        public SquadController(ISquadRepositorio squadRepository)
        {
            _squadRepository = squadRepository;
        }

        [HttpGet]
        [Route("Listar-Todos-Squads")]
        [Authorize(Roles = "Gerente,HelpDesk,RH")]
        public async Task<IActionResult> ListarSquads()
        {
            var squads = await _squadRepository.ListarSquads();
            return Ok(squads);
        }
        [HttpGet]
        [Route("Listar-Squad-Id/{id}")]
        [Authorize(Roles = "Gerente,HelpDesk,RH")]
        public async Task<IActionResult> BuscarSquadPorId(int id)
        {
            var (squad, usuarios) = await _squadRepository.BuscarSquadPorId(id);

            if (squad == null)
            {
                return NotFound();
            }
            return Ok(new { squad, usuarios });
        }

        [HttpGet]
        [Route("Listar-Usuarios-Squad/{id_squad}")]
        public async Task<IActionResult> BuscarUsuarios(int id_squad)
        {
            var usuarios = await _squadRepository.BuscarUsuarios(id_squad);
            if (usuarios == null || !usuarios.Any())
            {
                return NotFound($"Nenhum usuário encontrado para o Squad com ID: {id_squad}");
            }
            return Ok(usuarios);
        }
        //[HttpGet]
        //[Route("Listar-Equipamentos-Squad/{}")]

        [HttpPost]
        [Route("Adicionar-Squad")]
        [Authorize(Roles = "Gerente,HelpDesk,RH")]
        public async Task<IActionResult> AdicionarSquad([FromBody] Squad squad)
        {
            if (squad == null)
            {
                return BadRequest("Dados inválidos.");
            }
            var novoSquad = await _squadRepository.AdicionarSquad(squad);
            return CreatedAtAction(nameof(BuscarSquadPorId), new { id = novoSquad.Id }, novoSquad);
        }
        [HttpPut]
        [Route("Edit-Squad/{id}")]
        [Authorize(Roles = "Gerente,HelpDesk,RH")]
        public async Task<IActionResult> AtualizarSquad([FromBody] Squad squad, int id)
        {
            if (squad == null || id <= 0)
            {
                return BadRequest("Dados inválidos.");
            }
            var updatedSquad = await _squadRepository.AtualizarSquad(squad, id);
            if (updatedSquad == null)
            {
                return NotFound();
            }
            return Ok(updatedSquad);
        }
        [HttpDelete]
        [Route("Delete-Squad/{id}")]
        [Authorize(Roles = "Gerente,HelpDesk,RH")]
        public async Task<IActionResult> ExcluirSquad(int id)
        {
            var result = await _squadRepository.ExcluirSquad(id);
            return Ok(result);
        }
    }
}
